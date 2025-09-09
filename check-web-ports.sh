#!/bin/bash

# Script kiểm tra và xử lý port conflicts cho web services

echo "🔍 Checking web ports (80, 443) usage..."

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

echo "=== Checking Port 80 ==="
if netstat -tuln | grep -q ":80 "; then
    echo "Port 80: USED"
    echo "Services using port 80:"
    sudo netstat -tulpn | grep :80
    sudo lsof -i :80 2>/dev/null || echo "Could not get process details"
else
    echo "Port 80: FREE"
fi

echo ""
echo "=== Checking Port 443 ==="
if netstat -tuln | grep -q ":443 "; then
    echo "Port 443: USED"
    echo "Services using port 443:"
    sudo netstat -tulpn | grep :443
    sudo lsof -i :443 2>/dev/null || echo "Could not get process details"
else
    echo "Port 443: FREE"
fi

echo ""
echo "=== Web Server Services Status ==="
for service in nginx apache2 httpd; do
    if systemctl is-active --quiet $service 2>/dev/null; then
        echo "$service: RUNNING"
        systemctl status $service --no-pager -l
    else
        echo "$service: NOT RUNNING"
    fi
done

echo ""
echo "=== Docker Containers ==="
docker ps -a | grep -E "(nginx|apache|httpd)" || echo "No web server containers found"

echo ""
echo "=== Alternative Ports Available ==="
for port in 8080 8081 8443 9080 9443; do
    if netstat -tuln | grep -q ":$port "; then
        echo "Port $port: USED"
    else
        echo "Port $port: FREE"
    fi
done

echo ""
echo "=== SOLUTIONS ==="
print_note "Option 1: Use host nginx as reverse proxy (RECOMMENDED)"
print_note "  - Keep system nginx running"
print_note "  - Configure it to proxy to application containers"
print_note "  - Remove nginx container from docker-compose"

print_note ""
print_note "Option 2: Stop system nginx and use nginx container"
print_note "  - sudo systemctl stop nginx"
print_note "  - sudo systemctl disable nginx"
print_note "  - Use nginx container with ports 80/443"

print_note ""
print_note "Option 3: Use different ports for nginx container"
print_note "  - Map nginx container to ports 8080/8443"
print_note "  - Access via http://domain:8080 and https://domain:8443"

echo ""
echo "=== RECOMMENDED ACTIONS ==="
if systemctl is-active --quiet nginx 2>/dev/null; then
    print_warning "System nginx is running. Recommended: Use host nginx as reverse proxy"
    echo "Run: ./setup-host-nginx.sh"
elif systemctl is-active --quiet apache2 2>/dev/null || systemctl is-active --quiet httpd 2>/dev/null; then
    print_warning "Apache is running. Consider stopping it or using different ports"
else
    print_status "No system web server detected. You can use nginx container normally"
fi
