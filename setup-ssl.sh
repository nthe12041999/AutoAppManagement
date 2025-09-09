#!/bin/bash

# Script để setup SSL certificates cho tlsoftware.io.vn
# Chạy script này trên VPS trước khi deploy

DOMAIN="tlsoftware.io.vn"
API_DOMAIN="api.tlsoftware.io.vn"
EMAIL="admin@tlsoftware.io.vn"  # Thay bằng email của bạn

echo "🔒 Setting up SSL certificates for $DOMAIN"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
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

# Check if running as root
if [[ $EUID -eq 0 ]]; then
   print_error "This script should not be run as root"
   exit 1
fi

# Install certbot if not exists
if ! command -v certbot &> /dev/null; then
    print_status "Installing certbot..."
    sudo apt update
    sudo apt install -y certbot
fi

# Stop nginx if running (to free up port 80)
print_status "Stopping nginx temporarily..."
sudo systemctl stop nginx 2>/dev/null || true
docker stop autoapp_nginx 2>/dev/null || true

# Request certificates
print_status "Requesting SSL certificates..."
sudo certbot certonly \
    --standalone \
    --agree-tos \
    --no-eff-email \
    --email $EMAIL \
    -d $DOMAIN \
    -d www.$DOMAIN \
    -d $API_DOMAIN

if [[ $? -eq 0 ]]; then
    print_status "SSL certificates obtained successfully!"
    
    # Create ssl directory if not exists
    SSL_DIR="/opt/autoappmanagement/nginx/ssl"
    sudo mkdir -p $SSL_DIR
    
    # Copy certificates to nginx ssl directory
    print_status "Copying certificates to application directory..."
    sudo cp /etc/letsencrypt/live/$DOMAIN/fullchain.pem $SSL_DIR/tlsoftware.io.vn.crt
    sudo cp /etc/letsencrypt/live/$DOMAIN/privkey.pem $SSL_DIR/tlsoftware.io.vn.key
    
    # Set proper permissions
    sudo chown -R $USER:$USER $SSL_DIR
    sudo chmod 644 $SSL_DIR/tlsoftware.io.vn.crt
    sudo chmod 600 $SSL_DIR/tlsoftware.io.vn.key
    
    print_status "Certificates copied to: $SSL_DIR"
    
    # Create renewal script
    print_status "Creating certificate renewal script..."
    sudo tee /usr/local/bin/renew-autoapp-ssl.sh > /dev/null << EOF
#!/bin/bash
# Auto-renewal script for AutoAppManagement SSL certificates

echo "Renewing SSL certificates for $DOMAIN..."

# Stop nginx container
docker stop autoapp_nginx 2>/dev/null || true

# Renew certificates
certbot renew --standalone --quiet

# Copy renewed certificates
if [[ -f "/etc/letsencrypt/live/$DOMAIN/fullchain.pem" ]]; then
    cp /etc/letsencrypt/live/$DOMAIN/fullchain.pem $SSL_DIR/tlsoftware.io.vn.crt
    cp /etc/letsencrypt/live/$DOMAIN/privkey.pem $SSL_DIR/tlsoftware.io.vn.key
    
    # Set permissions
    chown $USER:$USER $SSL_DIR/tlsoftware.io.vn.*
    chmod 644 $SSL_DIR/tlsoftware.io.vn.crt
    chmod 600 $SSL_DIR/tlsoftware.io.vn.key
    
    echo "Certificates renewed and copied successfully!"
else
    echo "Failed to renew certificates!"
    exit 1
fi

# Restart nginx container
cd /opt/autoappmanagement
docker-compose -f docker-compose.prod.yml up -d nginx

echo "SSL renewal completed!"
EOF

    sudo chmod +x /usr/local/bin/renew-autoapp-ssl.sh
    
    # Setup cron job for auto-renewal
    print_status "Setting up automatic renewal..."
    (sudo crontab -l 2>/dev/null; echo "0 2 * * 0 /usr/local/bin/renew-autoapp-ssl.sh >> /var/log/ssl-renewal.log 2>&1") | sudo crontab -
    
    print_status "✅ SSL setup completed successfully!"
    print_status "Certificates are valid for 90 days and will auto-renew weekly"
    print_status "Renewal logs: /var/log/ssl-renewal.log"
    
else
    print_error "Failed to obtain SSL certificates!"
    print_warning "Make sure:"
    print_warning "1. Domain $DOMAIN points to this server's IP"
    print_warning "2. Port 80 is accessible from the internet"
    print_warning "3. No other web server is running on port 80"
    exit 1
fi

print_status "You can now run the deployment script:"
print_status "./deploy-vps.sh prod"
