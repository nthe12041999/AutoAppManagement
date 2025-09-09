#!/bin/bash

# Script tự động phát hiện và xử lý port conflicts

echo "🔧 AutoAppManagement Port Conflict Resolver"

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

print_status "Analyzing current port usage..."

# Check critical ports
PORT_80_USED=false
PORT_443_USED=false
PORT_1433_USED=false
NGINX_RUNNING=false
APACHE_RUNNING=false

if netstat -tuln | grep -q ":80 "; then
    PORT_80_USED=true
fi

if netstat -tuln | grep -q ":443 "; then
    PORT_443_USED=true
fi

if netstat -tuln | grep -q ":1433 "; then
    PORT_1433_USED=true
fi

if systemctl is-active --quiet nginx 2>/dev/null; then
    NGINX_RUNNING=true
fi

if systemctl is-active --quiet apache2 2>/dev/null || systemctl is-active --quiet httpd 2>/dev/null; then
    APACHE_RUNNING=true
fi

echo ""
echo "=== PORT ANALYSIS ==="
echo "Port 80 (HTTP): $([ "$PORT_80_USED" = true ] && echo "USED" || echo "FREE")"
echo "Port 443 (HTTPS): $([ "$PORT_443_USED" = true ] && echo "USED" || echo "FREE")"
echo "Port 1433 (SQL): $([ "$PORT_1433_USED" = true ] && echo "USED" || echo "FREE")"
echo "Nginx: $([ "$NGINX_RUNNING" = true ] && echo "RUNNING" || echo "NOT RUNNING")"
echo "Apache: $([ "$APACHE_RUNNING" = true ] && echo "RUNNING" || echo "NOT RUNNING")"

echo ""
echo "=== RECOMMENDED DEPLOYMENT STRATEGY ==="

# Determine best strategy
if [ "$PORT_1433_USED" = true ] && ([ "$PORT_80_USED" = true ] || [ "$PORT_443_USED" = true ]); then
    print_status "🎯 DETECTED: External SQL Server + Web Server Running"
    print_note "Best approach: Use host nginx as reverse proxy + external SQL"
    echo ""
    echo "Steps to deploy:"
    echo "1. ./setup-external-database.sh    # Setup database on existing SQL Server"
    echo "2. ./setup-host-nginx.sh           # Configure host nginx as reverse proxy" 
    echo "3. ./deploy-no-nginx.sh            # Deploy containers without nginx"
    
elif [ "$PORT_1433_USED" = true ]; then
    print_status "🎯 DETECTED: External SQL Server Only"
    print_note "Best approach: Use external SQL + nginx container"
    echo ""
    echo "Steps to deploy:"
    echo "1. ./setup-external-database.sh    # Setup database"
    echo "2. ./deploy-external-sql.sh        # Deploy with external SQL"
    
elif [ "$PORT_80_USED" = true ] || [ "$PORT_443_USED" = true ]; then
    print_status "🎯 DETECTED: Web Server Running"
    print_note "Best approach: Use host nginx as reverse proxy"
    echo ""
    echo "Steps to deploy:"
    echo "1. ./setup-host-nginx.sh           # Configure host nginx"
    echo "2. ./deploy-no-nginx.sh            # Deploy containers without nginx"
    
else
    print_status "🎯 DETECTED: Clean Environment"
    print_note "Best approach: Full containerized deployment"
    echo ""
    echo "Steps to deploy:"
    echo "1. ./setup-ssl.sh                  # Get SSL certificates"
    echo "2. ./deploy-vps.sh                 # Full deployment with all containers"
fi

echo ""
echo "=== REMOTE SQL SERVER OPTION ==="
print_note "🌐 For Remote SQL Server (125.253.121.206:1433):"
echo "1. ./test-remote-sql-connectivity.sh  # Test connectivity first"
echo "2. ./setup-remote-database.sh         # Setup remote database" 
echo "3. ./deploy-remote-sql.sh             # Deploy with remote SQL"

echo ""
echo "=== ALTERNATIVE OPTIONS ==="

if [ "$NGINX_RUNNING" = true ]; then
    print_warning "Option A: Stop system nginx and use containers"
    echo "  sudo systemctl stop nginx && sudo systemctl disable nginx"
    echo "  ./deploy-vps.sh"
fi

if [ "$PORT_80_USED" = true ] || [ "$PORT_443_USED" = true ]; then
    print_warning "Option B: Use alternative ports"
    echo "  Modify docker-compose to use ports 8080/8443"
    echo "  Access via http://domain:8080 and https://domain:8443"
fi

echo ""
print_note "=== CURRENT SERVICES USING PORTS ==="

if [ "$PORT_80_USED" = true ]; then
    echo "Port 80 used by:"
    sudo netstat -tulpn | grep :80 | head -3
fi

if [ "$PORT_443_USED" = true ]; then
    echo "Port 443 used by:"
    sudo netstat -tulpn | grep :443 | head -3
fi

if [ "$PORT_1433_USED" = true ]; then
    echo "Port 1433 used by:"
    sudo netstat -tulpn | grep :1433 | head -3
fi

echo ""
echo "=== QUICK ACTIONS ==="
read -p "Do you want to proceed with the recommended approach? (y/N): " -n 1 -r
echo

if [[ $REPLY =~ ^[Yy]$ ]]; then
    if [ "$PORT_1433_USED" = true ] && ([ "$PORT_80_USED" = true ] || [ "$PORT_443_USED" = true ]); then
        print_status "Executing: Host nginx + External SQL deployment"
        echo "Step 1: Setting up external database..."
        chmod +x setup-external-database.sh
        ./setup-external-database.sh
        
        if [ $? -eq 0 ]; then
            echo "Step 2: Setting up host nginx..."
            chmod +x setup-host-nginx.sh
            ./setup-host-nginx.sh
            
            if [ $? -eq 0 ]; then
                echo "Step 3: Deploying containers..."
                chmod +x deploy-no-nginx.sh
                ./deploy-no-nginx.sh
            fi
        fi
        
    elif [ "$PORT_1433_USED" = true ]; then
        print_status "Executing: External SQL deployment"
        chmod +x setup-external-database.sh
        ./setup-external-database.sh
        
        if [ $? -eq 0 ]; then
            chmod +x deploy-external-sql.sh
            ./deploy-external-sql.sh
        fi
        
    elif [ "$PORT_80_USED" = true ] || [ "$PORT_443_USED" = true ]; then
        print_status "Executing: Host nginx deployment"
        chmod +x setup-host-nginx.sh
        ./setup-host-nginx.sh
        
        if [ $? -eq 0 ]; then
            chmod +x deploy-no-nginx.sh
            ./deploy-no-nginx.sh
        fi
        
    else
        print_status "Executing: Full containerized deployment"
        chmod +x setup-ssl.sh deploy-vps.sh
        ./setup-ssl.sh
        
        if [ $? -eq 0 ]; then
            ./deploy-vps.sh
        fi
    fi
else
    print_note "Manual deployment cancelled. You can run individual scripts as needed."
fi

echo ""
print_status "=== AVAILABLE SCRIPTS ==="
echo "setup-external-database.sh  - Setup database on existing SQL Server"
echo "setup-host-nginx.sh         - Configure host nginx as reverse proxy"
echo "setup-ssl.sh                - Get SSL certificates with Let's Encrypt"
echo "deploy-external-sql.sh      - Deploy with external SQL Server"
echo "deploy-no-nginx.sh          - Deploy containers without nginx"
echo "deploy-vps.sh               - Full containerized deployment"
echo "check-web-ports.sh          - Check web ports usage"
echo "check-ports.sh              - Check all ports usage"
