#!/bin/bash

# Debug script để tìm và giải quyết vấn đề nginx

echo "🔧 AutoAppManagement Debug Script"

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

echo "=== SYSTEM STATUS ==="
echo "Date: $(date)"
echo "User: $(whoami)"
echo "PWD: $(pwd)"
echo ""

echo "=== NGINX STATUS ==="
if systemctl is-active --quiet nginx; then
    print_status "Nginx is running"
    echo "Nginx version: $(nginx -v 2>&1)"
    echo "Nginx config test:"
    sudo nginx -t 2>&1
else
    print_warning "Nginx is not running"
    echo "Trying to start nginx..."
    sudo systemctl start nginx
    sudo systemctl enable nginx
fi

echo ""
echo "=== PORT USAGE ==="
echo "Port 80:"
sudo netstat -tulpn | grep ":80 " || echo "Port 80 is free"
echo "Port 443:"
sudo netstat -tulpn | grep ":443 " || echo "Port 443 is free"
echo "Port 8080:"
sudo netstat -tulpn | grep ":8080 " || echo "Port 8080 is free"
echo "Port 8081:"
sudo netstat -tulpn | grep ":8081 " || echo "Port 8081 is free"

echo ""
echo "=== NGINX CONFIGURATION ==="
echo "Sites available:"
ls -la /etc/nginx/sites-available/ 2>/dev/null || echo "Directory not found"
echo ""
echo "Sites enabled:"
ls -la /etc/nginx/sites-enabled/ 2>/dev/null || echo "Directory not found"

echo ""
echo "=== AUTOAPP NGINX CONFIG ==="
if [[ -f "/etc/nginx/sites-available/autoappmanagement" ]]; then
    print_status "AutoApp nginx config exists"
    echo "Config file size: $(wc -l /etc/nginx/sites-available/autoappmanagement)"
else
    print_warning "AutoApp nginx config does not exist"
fi

if [[ -f "/etc/nginx/sites-enabled/autoappmanagement" ]]; then
    print_status "AutoApp nginx config is enabled"
else
    print_warning "AutoApp nginx config is not enabled"
fi

echo ""
echo "=== SSL CERTIFICATES ==="
if [[ -d "/etc/nginx/ssl" ]]; then
    echo "SSL directory exists:"
    ls -la /etc/nginx/ssl/
else
    print_warning "SSL directory does not exist"
fi

echo ""
echo "=== DOCKER STATUS ==="
if command -v docker &> /dev/null; then
    print_status "Docker is available"
    echo "Docker version: $(docker --version)"
    echo "Running containers:"
    docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
else
    print_error "Docker is not available"
fi

echo ""
echo "=== AUTOAPP FILES ==="
echo "Current directory files:"
ls -la *.yml *.sh .env* 2>/dev/null

echo ""
echo "=== REMOTE SQL CONFIG ==="
if [[ -f ".env.remote-sql" ]]; then
    print_status "Remote SQL config exists"
    echo "Connection string configured: $(head -c 50 .env.remote-sql)..."
else
    print_warning "Remote SQL config does not exist"
fi

echo ""
echo "=== SUGGESTED ACTIONS ==="

# Check what's blocking ports 80/443
BLOCKING_80=$(sudo lsof -i :80 2>/dev/null | grep LISTEN | head -1)
BLOCKING_443=$(sudo lsof -i :443 2>/dev/null | grep LISTEN | head -1)

if [[ -n "$BLOCKING_80" ]]; then
    print_warning "Port 80 blocked by: $BLOCKING_80"
fi

if [[ -n "$BLOCKING_443" ]]; then
    print_warning "Port 443 blocked by: $BLOCKING_443"
fi

# Provide solutions
echo ""
echo "=== SOLUTIONS ==="
echo "Option 1: Fix nginx host setup"
echo "  sudo systemctl stop nginx"
echo "  ./setup-host-nginx.sh"
echo "  sudo systemctl start nginx"
echo ""
echo "Option 2: Use alternative ports"
echo "  docker-compose -f docker-compose.remote-sql-alt-ports.yml up -d --build"
echo ""
echo "Option 3: Stop conflicting services"
if [[ -n "$BLOCKING_80" ]] || [[ -n "$BLOCKING_443" ]]; then
    echo "  sudo systemctl stop nginx apache2 httpd 2>/dev/null"
    echo "  ./deploy-remote-sql.sh"
fi

echo ""
echo "=== QUICK FIX ==="
print_status "To deploy immediately with alternative ports:"
echo "export DB_CONNECTION_STRING=\"\$(cat .env.remote-sql | grep DB_CONNECTION_STRING | cut -d'\"' -f2)\""
echo "docker-compose -f docker-compose.remote-sql-alt-ports.yml up -d --build"
echo ""
echo "Then access:"
echo "Web App: http://\$(curl -s ifconfig.me):8082"
echo "API: http://\$(curl -s ifconfig.me):8081"
