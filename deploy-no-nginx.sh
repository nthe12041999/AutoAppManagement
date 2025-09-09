#!/bin/bash

# Deploy script cho AutoAppManagement với host nginx
# Usage: ./deploy-no-nginx.sh [environment]

set -e

ENVIRONMENT=${1:-prod}
PROJECT_NAME="autoappmanagement"
DOMAIN="tlsoftware.io.vn"
API_DOMAIN="api.tlsoftware.io.vn"
VERSION=$(date +%Y%m%d-%H%M%S)

echo "🚀 Starting deployment WITHOUT nginx container for: $DOMAIN"

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

# Check if database is setup
if [[ ! -f ".env.external-sql" ]]; then
    print_error "Database not setup! Please run setup-external-database.sh first"
    exit 1
fi

# Load database connection string
source .env.external-sql

if [[ -z "$DB_CONNECTION_STRING" ]]; then
    print_error "DB_CONNECTION_STRING not found!"
    exit 1
fi

print_status "Database connection configured ✅"

# Check Docker
if ! command -v docker &> /dev/null || ! command -v docker-compose &> /dev/null; then
    print_error "Docker or Docker Compose not installed!"
    exit 1
fi

# Check if host nginx is configured
if ! systemctl is-active --quiet nginx; then
    print_warning "Host nginx is not running!"
    print_note "Please run: ./setup-host-nginx.sh"
    read -p "Continue anyway? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

COMPOSE_FILE="docker-compose.no-nginx.yml"

# Export environment variables
export DB_CONNECTION_STRING
export DOMAIN
export API_DOMAIN

print_status "Environment: $ENVIRONMENT"
print_status "Using compose file: $COMPOSE_FILE"
print_status "Domain: $DOMAIN (via host nginx)"
print_status "API Domain: $API_DOMAIN (via host nginx)"

# Check if ports 8080/8081 are free
print_status "Checking application ports..."
for port in 8080 8081; do
    if netstat -tuln | grep -q ":$port "; then
        print_error "Port $port is already in use!"
        print_status "Services using port $port:"
        sudo netstat -tulpn | grep :$port
        exit 1
    fi
done

# Stop existing containers
print_status "Stopping existing containers..."
docker-compose -f $COMPOSE_FILE down --remove-orphans 2>/dev/null || true
docker-compose -f docker-compose.external-sql.yml down --remove-orphans 2>/dev/null || true

# Clean up images
print_status "Cleaning up old images..."
docker image prune -f

# Build and start containers
print_status "Building and starting containers..."
docker-compose -f $COMPOSE_FILE up -d --build

# Wait for services to be healthy
print_status "Waiting for services to be ready..."
sleep 45

# Check if services are running
print_status "Checking service health..."
if docker-compose -f $COMPOSE_FILE ps | grep -q "unhealthy\|Exit"; then
    print_error "Some services are not healthy!"
    print_status "Container logs:"
    docker-compose -f $COMPOSE_FILE logs --tail=30
    exit 1
else
    print_status "✅ All containers are running healthy!"
fi

# Test local endpoints
print_status "Testing local endpoints..."
sleep 10

# Test webapp
if curl -f http://localhost:8080/health >/dev/null 2>&1; then
    print_status "✅ WebApp (localhost:8080) is responding"
else
    print_warning "⚠️ WebApp (localhost:8080) is not responding"
fi

# Test API
if curl -f http://localhost:8081/health >/dev/null 2>&1; then
    print_status "✅ API (localhost:8081) is responding"
else
    print_warning "⚠️ API (localhost:8081) is not responding"
fi

# Run database migrations
print_status "Running database migrations..."
docker-compose -f $COMPOSE_FILE exec -T api dotnet ef database update || print_warning "Migration might have failed"

# Test nginx proxy
print_status "Testing nginx proxy..."
if systemctl is-active --quiet nginx; then
    # Test if nginx can reach the apps
    if curl -f http://localhost:8080 >/dev/null 2>&1; then
        print_status "✅ Nginx → WebApp proxy should work"
    else
        print_warning "⚠️ Nginx → WebApp proxy might have issues"
    fi
    
    if curl -f http://localhost:8081 >/dev/null 2>&1; then
        print_status "✅ Nginx → API proxy should work"
    else
        print_warning "⚠️ Nginx → API proxy might have issues"
    fi
else
    print_warning "Host nginx is not running"
fi

# Create monitoring script for no-nginx setup
print_status "Creating monitoring script..."
cat > monitor-no-nginx.sh << EOF
#!/bin/bash
echo "=== AutoAppManagement Health Check (No Nginx Container) ==="
echo "Date: \$(date)"
echo ""

echo "=== Container Status ==="
docker-compose -f $COMPOSE_FILE ps

echo ""
echo "=== Local Endpoints ==="
echo "Testing localhost:8080 (WebApp):"
curl -sL -w "Response: %{http_code}\\n" "http://localhost:8080/health" -o /dev/null 2>/dev/null || echo "Connection failed"

echo "Testing localhost:8081 (API):"
curl -sL -w "Response: %{http_code}\\n" "http://localhost:8081/health" -o /dev/null 2>/dev/null || echo "Connection failed"

echo ""
echo "=== Host Nginx Status ==="
if systemctl is-active --quiet nginx; then
    echo "Nginx: RUNNING"
    echo "Nginx config test:"
    sudo nginx -t 2>&1 || echo "Config test failed"
else
    echo "Nginx: NOT RUNNING"
fi

echo ""
echo "=== Public URLs ==="
echo "Testing https://$DOMAIN:"
curl -sL -w "Response: %{http_code}\\n" "https://$DOMAIN" -o /dev/null 2>/dev/null || echo "Connection failed"

echo "Testing https://$API_DOMAIN/health:"
curl -sL -w "Response: %{http_code}\\n" "https://$API_DOMAIN/health" -o /dev/null 2>/dev/null || echo "Connection failed"

echo ""
echo "=== System Resources ==="
echo "Memory:"
free -h | head -2

echo "Disk:"
df -h / | tail -1

echo ""
echo "=== Port Usage ==="
echo "Port 8080 (WebApp): \$(netstat -tuln | grep :8080 | wc -l) connections"
echo "Port 8081 (API): \$(netstat -tuln | grep :8081 | wc -l) connections"
echo "Port 80 (HTTP): \$(netstat -tuln | grep :80 | wc -l) connections"
echo "Port 443 (HTTPS): \$(netstat -tuln | grep :443 | wc -l) connections"
EOF

chmod +x monitor-no-nginx.sh

print_status "✅ Deployment completed successfully! 🎉"
print_status ""
print_status "=== DEPLOYMENT SUMMARY ==="
print_status "Architecture: Host Nginx + Docker Containers"
print_status "WebApp Container: localhost:8080"
print_status "API Container: localhost:8081"
print_status "Public Web App: https://$DOMAIN"
print_status "Public API: https://$API_DOMAIN"
print_status ""
print_status "=== USEFUL COMMANDS ==="
print_status "Container logs: docker-compose -f $COMPOSE_FILE logs -f"
print_status "Restart containers: docker-compose -f $COMPOSE_FILE restart"
print_status "Stop containers: docker-compose -f $COMPOSE_FILE down"
print_status "Monitor system: ./monitor-no-nginx.sh"
print_status "Nginx logs: sudo tail -f /var/log/nginx/access.log"
print_status "Nginx reload: sudo systemctl reload nginx"
print_status ""
print_status "=== ARCHITECTURE ==="
print_note "Internet → Host Nginx (80/443) → Containers (8080/8081)"
print_note "SSL termination: Host Nginx"
print_note "Load balancing: Host Nginx"
print_note "Database: External SQL Server"

# Show running containers
print_status ""
print_status "Running containers:"
docker-compose -f $COMPOSE_FILE ps

# Final health check
print_status ""
print_status "Running health check..."
./monitor-no-nginx.sh
