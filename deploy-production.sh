#!/bin/bash

# AutoAppManagement Deployment Script for tlsoftware.io.vn
# Run this script from /opt/autoappmanagement directory

echo "🚀 Starting AutoAppManagement Deployment..."

# Check if running as root or sudo
if [[ $EUID -ne 0 ]]; then
   echo "❌ This script must be run as root or with sudo"
   exit 1
fi

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8 SDK not found. Installing..."
    
    # Download and install Microsoft package signing key
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    # Update packages and install .NET 8 SDK
    apt-get update
    apt-get install -y apt-transport-https
    apt-get update
    apt-get install -y dotnet-sdk-8.0
    
    # Verify installation
    if ! command -v dotnet &> /dev/null; then
        echo "❌ Failed to install .NET 8 SDK"
        exit 1
    fi
    
    echo "✅ .NET 8 SDK installed successfully"
    dotnet --version
fi

# Set variables
PROJECT_DIR="/opt/autoappmanagement"
WEBAPP_DIR="$PROJECT_DIR/AutoAppManagement"
API_DIR="$PROJECT_DIR/AutoAppManagement.API"
PUBLISH_DIR="/opt/autoappmanagement-published"
SERVICE_NAME="autoappmanagement"
DOMAIN="tlsoftware.io.vn"

echo "📂 Working Directory: $PROJECT_DIR"
echo "🌐 Domain: $DOMAIN"
echo "🔧 .NET Version: $(dotnet --version)"

# Navigate to project directory
cd $PROJECT_DIR

# Pull latest code
echo "📥 Pulling latest code from Git..."
git pull origin main

# Stop existing services
echo "⏸️  Stopping existing services..."
systemctl stop $SERVICE_NAME-webapp || echo "WebApp service not running"
systemctl stop $SERVICE_NAME-api || echo "API service not running"


# Clean previous builds
echo "🧹 Cleaning previous builds..."
rm -rf $PUBLISH_DIR
mkdir -p $PUBLISH_DIR

# Build and publish Web App
echo "🔨 Building Web Application..."
cd $WEBAPP_DIR
dotnet clean
dotnet restore
dotnet build -c Release

if [ $? -ne 0 ]; then
    echo "❌ Web App build failed"
    exit 1
fi

dotnet publish -c Release -o $PUBLISH_DIR/webapp --self-contained false

if [ $? -ne 0 ]; then
    echo "❌ Web App publish failed"
    exit 1
fi

# Build and publish API
echo "🔨 Building API Application..."
cd $API_DIR
dotnet clean
dotnet restore
dotnet build -c Release

if [ $? -ne 0 ]; then
    echo "❌ API build failed"
    exit 1
fi

dotnet publish -c Release -o $PUBLISH_DIR/api --self-contained false

if [ $? -ne 0 ]; then
    echo "❌ API publish failed"
    exit 1
fi

# Copy production config and database
echo "⚙️  Copying production configuration..."
cp $PROJECT_DIR/appsettings.Production.json $PUBLISH_DIR/webapp/
cp $PROJECT_DIR/appsettings.Production.json $PUBLISH_DIR/api/

# Copy database to published directory
echo "📋 Copying database..."
mkdir -p $PUBLISH_DIR/database
cp -r $PROJECT_DIR/database/* $PUBLISH_DIR/database/ 2>/dev/null || echo "No database to copy"

# Set permissions
echo "🔐 Setting permissions..."
chown -R www-data:www-data $PUBLISH_DIR
chmod -R 755 $PUBLISH_DIR
chmod 644 $PUBLISH_DIR/database/AutoAppManagement.db 2>/dev/null || echo "No database file to set permissions"

# Create systemd service for Web App
echo "📝 Creating systemd service for Web App..."
cat > /etc/systemd/system/$SERVICE_NAME-webapp.service << EOF
[Unit]
Description=AutoAppManagement Web Application
After=network.target

[Service]
WorkingDirectory=$PUBLISH_DIR/webapp
ExecStart=/usr/bin/dotnet $PUBLISH_DIR/webapp/AutoAppManagement.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=$SERVICE_NAME-webapp
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
EOF

# Create systemd service for API
echo "📝 Creating systemd service for API..."
cat > /etc/systemd/system/$SERVICE_NAME-api.service << EOF
[Unit]
Description=AutoAppManagement API
After=network.target

[Service]
WorkingDirectory=$PUBLISH_DIR/api
ExecStart=/usr/bin/dotnet $PUBLISH_DIR/api/AutoAppManagement.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=$SERVICE_NAME-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5001

[Install]
WantedBy=multi-user.target
EOF

# Reload systemd and enable services
echo "⚡ Enabling services..."
systemctl daemon-reload
systemctl enable $SERVICE_NAME-webapp
systemctl enable $SERVICE_NAME-api

# Start services
echo "▶️  Starting services..."
systemctl start $SERVICE_NAME-webapp
systemctl start $SERVICE_NAME-api

# Wait and check status
sleep 5
echo "📊 Service Status:"
systemctl status $SERVICE_NAME-webapp --no-pager -l
echo "---"
systemctl status $SERVICE_NAME-api --no-pager -l

# Configure Nginx
echo "🌐 Configuring Nginx..."
cat > /etc/nginx/sites-available/$DOMAIN << EOF
server {
    listen 80;
    server_name $DOMAIN www.$DOMAIN;
    
    # Security headers
    add_header X-Content-Type-Options nosniff;
    add_header X-Frame-Options DENY;
    add_header X-XSS-Protection "1; mode=block";
    
    # Main application
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
    
    # API endpoints
    location /api/ {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        
        # API specific settings
        client_max_body_size 50M;
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
    
    # Static files
    location ~* \.(css|js|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        proxy_pass http://localhost:5000;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
    
    # Health check
    location /health {
        access_log off;
        return 200 "healthy";
        add_header Content-Type text/plain;
    }
}
EOF

# Enable nginx site
ln -sf /etc/nginx/sites-available/$DOMAIN /etc/nginx/sites-enabled/
rm -f /etc/nginx/sites-enabled/default

# Test nginx configuration
echo "🔧 Testing Nginx configuration..."
nginx -t
if [ $? -eq 0 ]; then
    echo "✅ Nginx configuration is valid"
    systemctl reload nginx
else
    echo "❌ Nginx configuration error"
    exit 1
fi

# Create log directories
mkdir -p /var/log/autoappmanagement
chown www-data:www-data /var/log/autoappmanagement

echo "🎉 Deployment completed successfully!"
echo ""
echo "📋 Summary:"
echo "   • Domain: http://$DOMAIN"
echo "   • Web App: http://localhost:5000" 
echo "   • API: http://localhost:5001"
echo "   • Published to: $PUBLISH_DIR"
echo "   • Database: SQL Server at 125.253.121.206"
echo ""
echo "🔧 Useful commands:"
echo "   • Check Web App: systemctl status $SERVICE_NAME-webapp"
echo "   • Check API: systemctl status $SERVICE_NAME-api"
echo "   • View Web logs: journalctl -u $SERVICE_NAME-webapp -f"
echo "   • View API logs: journalctl -u $SERVICE_NAME-api -f"
echo "   • Restart Web: systemctl restart $SERVICE_NAME-webapp"
echo "   • Restart API: systemctl restart $SERVICE_NAME-api"
echo ""

# Final health check
echo "🏥 Performing health check..."
sleep 10

# Test web app
if curl -s -I http://localhost:5000 | grep "HTTP" > /dev/null; then
    echo "✅ Web App is responding"
else
    echo "⚠️  Web App may not be ready yet - checking logs..."
    echo "Last 10 lines of Web App logs:"
    journalctl -u $SERVICE_NAME-webapp -n 10 --no-pager
fi

# Test API
if curl -s -I http://localhost:5001/api 2>/dev/null | grep "HTTP" > /dev/null; then
    echo "✅ API is responding"
else
    echo "⚠️  API may not be ready yet - checking logs..."
    echo "Last 10 lines of API logs:"
    journalctl -u $SERVICE_NAME-api -n 10 --no-pager
fi

# Test domain
if curl -s -I http://$DOMAIN 2>/dev/null | grep "HTTP" > /dev/null; then
    echo "✅ Domain is accessible"
else
    echo "⚠️  Domain may not be accessible yet"
    echo "   Make sure DNS A record points $DOMAIN to $(curl -s ifconfig.me)"
fi

echo ""
echo "🚀 AutoAppManagement is now deployed!"
echo "🌐 Access your application at: http://$DOMAIN"
echo "👤 Default admin login: admin / 123456"
echo ""
echo "📱 Next steps:"
echo "   1. Verify DNS A record: $DOMAIN → $(curl -s ifconfig.me)"
echo "   2. Test login at: http://$DOMAIN/Auth/Login"  
echo "   3. Configure SSL certificate (recommended)"
echo "   4. Change default admin password"