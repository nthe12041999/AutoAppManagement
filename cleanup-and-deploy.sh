#!/bin/bash

# Script cleanup containers cũ và deploy lại

echo "🧹 Cleaning up existing containers and ports..."

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

# Stop all AutoApp related containers
print_status "Stopping all AutoApp containers..."
docker stop autoapp_webapp autoapp_api autoapp_nginx autoapp_sqlserver 2>/dev/null || true

# Remove all AutoApp related containers
print_status "Removing all AutoApp containers..."
docker rm autoapp_webapp autoapp_api autoapp_nginx autoapp_sqlserver 2>/dev/null || true

# Stop any docker-compose services
print_status "Stopping docker-compose services..."
docker-compose -f docker-compose.prod.yml down --remove-orphans 2>/dev/null || true
docker-compose -f docker-compose.yml down --remove-orphans 2>/dev/null || true

# Check if port 1433 is still in use
print_status "Checking port 1433..."
if netstat -tuln | grep -q ":1433 "; then
    print_warning "Port 1433 is still in use by another service:"
    sudo netstat -tulpn | grep :1433
    echo ""
    print_warning "You have two options:"
    echo "1. Use docker-compose.prod.yml (no external SQL port - recommended)"
    echo "2. Use docker-compose.prod-alt.yml (SQL on port 14330)"
    echo ""
    read -p "Which option? (1/2): " choice
    
    if [[ $choice == "2" ]]; then
        COMPOSE_FILE="docker-compose.prod-alt.yml"
        DB_CONNECTION_STRING="Server=sqlserver,1433;Database=AutoAppManagement;User Id=sa;Password=AutoApp@Production@2024;TrustServerCertificate=true;Encrypt=false;"
        print_status "Using alternative compose file with port 14330"
    else
        COMPOSE_FILE="docker-compose.prod.yml"
        DB_CONNECTION_STRING="Server=sqlserver,1433;Database=AutoAppManagement;User Id=sa;Password=AutoApp@Production@2024;TrustServerCertificate=true;Encrypt=false;"
        print_status "Using standard compose file (no external SQL port)"
    fi
else
    COMPOSE_FILE="docker-compose.prod.yml"
    DB_CONNECTION_STRING="Server=sqlserver,1433;Database=AutoAppManagement;User Id=sa;Password=AutoApp@Production@2024;TrustServerCertificate=true;Encrypt=false;"
    print_status "Port 1433 is free, using standard compose file"
fi

# Export environment variable
export DB_CONNECTION_STRING

# Clean up images (optional)
print_status "Cleaning up unused images..."
docker image prune -f

# Pull/Build and start services
print_status "Starting services with $COMPOSE_FILE..."
docker-compose -f $COMPOSE_FILE up -d --build

# Wait for services
print_status "Waiting for services to start..."
sleep 60

# Check status
print_status "Checking service status..."
docker-compose -f $COMPOSE_FILE ps

# Check health
print_status "Running health checks..."
sleep 30

if docker-compose -f $COMPOSE_FILE ps | grep -q "unhealthy\|Exit"; then
    print_error "Some services are not healthy!"
    print_status "Container logs:"
    docker-compose -f $COMPOSE_FILE logs --tail=50
    exit 1
else
    print_status "All services are running healthy!"
fi

# Run migrations
print_status "Running database migrations..."
docker-compose -f $COMPOSE_FILE exec -T api dotnet ef database update || print_warning "Migration might have failed"

print_status "✅ Deployment completed successfully!"
print_status ""
print_status "=== Service URLs ==="
print_status "Web App: https://tlsoftware.io.vn"
print_status "API: https://api.tlsoftware.io.vn"

if [[ $COMPOSE_FILE == "docker-compose.prod-alt.yml" ]]; then
    print_status "SQL Server: localhost:14330"
else
    print_status "SQL Server: Internal only (port not exposed)"
fi

print_status ""
print_status "=== Useful Commands ==="
print_status "View logs: docker-compose -f $COMPOSE_FILE logs -f"
print_status "Check status: docker-compose -f $COMPOSE_FILE ps"
print_status "Connect to SQL: docker exec -it autoapp_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa"
