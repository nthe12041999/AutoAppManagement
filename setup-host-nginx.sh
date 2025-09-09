#!/bin/bash

# Script setup nginx host làm reverse proxy cho AutoAppManagement containers

echo "🌐 Setting up host nginx as reverse proxy for AutoAppManagement..."

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_note() {
    echo -e "${BLUE}[NOTE]${NC} $1"
}

DOMAIN="tlsoftware.io.vn"
API_DOMAIN="api.tlsoftware.io.vn"

# Check if nginx is installed
if ! command -v nginx &> /dev/null; then
    print_status "Installing nginx..."
    if [[ -f /etc/debian_version ]]; then
        sudo apt update
        sudo apt install -y nginx
    elif [[ -f /etc/redhat-release ]]; then
        sudo yum install -y nginx
    else
        print_error "Unsupported OS. Please install nginx manually."
        exit 1
    fi
fi

# Backup existing nginx config
if [[ -f "/etc/nginx/sites-available/default" ]]; then
    print_status "Backing up existing nginx config..."
    sudo cp /etc/nginx/sites-available/default /etc/nginx/sites-available/default.backup.$(date +%Y%m%d_%H%M%S)
fi

# Create nginx config for AutoAppManagement
print_status "Creating nginx configuration for $DOMAIN..."

sudo tee /etc/nginx/sites-available/autoappmanagement << EOF
# AutoAppManagement Nginx Configuration
# Main website
server {
    listen 80;
    server_name $DOMAIN www.$DOMAIN;
    
    # Redirect HTTP to HTTPS
    return 301 https://\$server_name\$request_uri;
}

server {
    listen 443 ssl http2;
    server_name $DOMAIN www.$DOMAIN;

    # SSL Configuration
    ssl_certificate /etc/nginx/ssl/tlsoftware.io.vn.crt;
    ssl_certificate_key /etc/nginx/ssl/tlsoftware.io.vn.key;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
    add_header Content-Security-Policy "default-src 'self' http: https: data: blob: 'unsafe-inline'" always;

    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_comp_level 6;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;

    # Rate limiting
    limit_req_zone \$binary_remote_addr zone=web:10m rate=30r/s;
    limit_req zone=web burst=50 nodelay;

    location / {
        proxy_pass http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        proxy_buffering off;
        proxy_read_timeout 300s;
        proxy_connect_timeout 75s;
        
        # Handle WebSocket connections
        proxy_set_header Connection "upgrade";
        proxy_set_header Upgrade \$http_upgrade;
    }

    # Health check endpoint
    location /health {
        access_log off;
        proxy_pass http://127.0.0.1:8080/health;
    }

    # Static files optimization
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|woff|woff2|ttf|svg)$ {
        proxy_pass http://127.0.0.1:8080;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}

# API subdomain
server {
    listen 80;
    server_name $API_DOMAIN;
    
    # Redirect HTTP to HTTPS
    return 301 https://\$server_name\$request_uri;
}

server {
    listen 443 ssl http2;
    server_name $API_DOMAIN;

    # SSL Configuration (same as main site)
    ssl_certificate /etc/nginx/ssl/tlsoftware.io.vn.crt;
    ssl_certificate_key /etc/nginx/ssl/tlsoftware.io.vn.key;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;

    # CORS headers for API
    add_header Access-Control-Allow-Origin "https://$DOMAIN" always;
    add_header Access-Control-Allow-Methods "GET, POST, PUT, DELETE, OPTIONS" always;
    add_header Access-Control-Allow-Headers "DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range,Authorization" always;
    add_header Access-Control-Expose-Headers "Content-Length,Content-Range" always;

    # Rate limiting for API
    limit_req_zone \$binary_remote_addr zone=api:10m rate=10r/s;
    limit_req zone=api burst=20 nodelay;

    location / {
        # Handle CORS preflight requests
        if (\$request_method = 'OPTIONS') {
            add_header Access-Control-Allow-Origin "https://$DOMAIN";
            add_header Access-Control-Allow-Methods "GET, POST, PUT, DELETE, OPTIONS";
            add_header Access-Control-Allow-Headers "DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range,Authorization";
            add_header Access-Control-Max-Age 1728000;
            add_header Content-Type "text/plain; charset=utf-8";
            add_header Content-Length 0;
            return 204;
        }

        proxy_pass http://127.0.0.1:8081;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        proxy_buffering off;
        proxy_read_timeout 300s;
        proxy_connect_timeout 75s;
    }

    # Health check endpoint
    location /health {
        access_log off;
        proxy_pass http://127.0.0.1:8081/health;
    }
}
EOF

# Create SSL directory
print_status "Creating SSL directory..."
sudo mkdir -p /etc/nginx/ssl

# Copy SSL certificates if they exist in project
if [[ -f "nginx/ssl/tlsoftware.io.vn.crt" ]] && [[ -f "nginx/ssl/tlsoftware.io.vn.key" ]]; then
    print_status "Copying SSL certificates..."
    sudo cp nginx/ssl/tlsoftware.io.vn.crt /etc/nginx/ssl/
    sudo cp nginx/ssl/tlsoftware.io.vn.key /etc/nginx/ssl/
    sudo chmod 644 /etc/nginx/ssl/tlsoftware.io.vn.crt
    sudo chmod 600 /etc/nginx/ssl/tlsoftware.io.vn.key
else
    print_warning "SSL certificates not found in nginx/ssl/"
    print_note "You need to either:"
    print_note "1. Run setup-ssl.sh to get Let's Encrypt certificates"
    print_note "2. Manually place certificates in /etc/nginx/ssl/"
fi

# Enable the site
print_status "Enabling AutoAppManagement site..."
sudo ln -sf /etc/nginx/sites-available/autoappmanagement /etc/nginx/sites-enabled/

# Disable default site
if [[ -f "/etc/nginx/sites-enabled/default" ]]; then
    print_status "Disabling default nginx site..."
    sudo rm /etc/nginx/sites-enabled/default
fi

# Test nginx configuration
print_status "Testing nginx configuration..."
sudo nginx -t

if [[ $? -eq 0 ]]; then
    print_status "✅ Nginx configuration is valid"
    
    # Restart nginx
    print_status "Restarting nginx..."
    sudo systemctl restart nginx
    sudo systemctl enable nginx
    
    print_status "✅ Nginx configured successfully!"
else
    print_error "❌ Nginx configuration test failed!"
    print_note "Please check the configuration and fix any errors"
    exit 1
fi

print_status ""
print_status "=== NGINX SETUP COMPLETED ==="
print_status "Configuration file: /etc/nginx/sites-available/autoappmanagement"
print_status "SSL certificates: /etc/nginx/ssl/"
print_status ""
print_status "=== PROXY CONFIGURATION ==="
print_status "Web App: $DOMAIN → localhost:8080"
print_status "API: $API_DOMAIN → localhost:8081"
print_status ""
print_status "=== NEXT STEPS ==="
print_note "1. Make sure SSL certificates are in place"
print_note "2. Deploy containers without nginx: ./deploy-no-nginx.sh"
print_note "3. Test the websites"
print_status ""
print_status "=== USEFUL COMMANDS ==="
print_status "Check nginx status: sudo systemctl status nginx"
print_status "Reload nginx: sudo systemctl reload nginx"
print_status "View nginx logs: sudo tail -f /var/log/nginx/error.log"
print_status "Test nginx config: sudo nginx -t"
