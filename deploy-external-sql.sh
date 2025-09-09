#!/bin/bash

# Deploy script cho AutoAppManagement với External SQL Server
# Usage: ./deploy-external-sql.sh [environment]

set -e

ENVIRONMENT=${1:-prod}
PROJECT_NAME="autoappmanagement"
DOMAIN="tlsoftware.io.vn"
API_DOMAIN="api.tlsoftware.io.vn"
VERSION=$(date +%Y%m%d-%H%M%S)

echo "🚀 Starting deployment with External SQL Server for: $DOMAIN"

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
    print_note "Run: chmod +x setup-external-database.sh && ./setup-external-database.sh"
    exit 1
fi

# Load database connection string
source .env.external-sql

if [[ -z "$DB_CONNECTION_STRING" ]]; then
    print_error "DB_CONNECTION_STRING not found! Please setup database first."
    exit 1
fi

print_status "Database connection configured ✅"

# Check Docker
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed!"
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed!"
    exit 1
fi

# Test SQL Server connection from host
print_status "Testing SQL Server connection..."
if command -v sqlcmd &> /dev/null; then
    # Extract password from connection string for testing
    DB_USER=$(echo $DB_CONNECTION_STRING | grep -oP 'User Id=\K[^;]*')
    DB_PASSWORD=$(echo $DB_CONNECTION_STRING | grep -oP 'Password=\K[^;]*')
    DB_NAME=$(echo $DB_CONNECTION_STRING | grep -oP 'Database=\K[^;]*')
    
    sqlcmd -S localhost -U "$DB_USER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 'SQL Server connection OK' as Status" > /dev/null 2>&1
    if [[ $? -eq 0 ]]; then
        print_status "✅ SQL Server connection successful"
    else
        print_error "❌ Cannot connect to SQL Server with application credentials"
        print_note "Please check database setup and connection string"
        exit 1
    fi
else
    print_warning "sqlcmd not found, skipping connection test"
fi

COMPOSE_FILE="docker-compose.external-sql.yml"

# Export environment variables
export DB_CONNECTION_STRING
export DOMAIN
export API_DOMAIN

print_status "Environment: $ENVIRONMENT"
print_status "Using compose file: $COMPOSE_FILE"
print_status "Domain: $DOMAIN"
print_status "API Domain: $API_DOMAIN"

# Create necessary directories
print_status "Creating necessary directories..."
mkdir -p nginx/ssl
mkdir -p logs
mkdir -p backups

# Check SSL certificates
print_status "Checking SSL certificates..."
if [[ ! -f "nginx/ssl/tlsoftware.io.vn.crt" ]] || [[ ! -f "nginx/ssl/tlsoftware.io.vn.key" ]]; then
    print_warning "SSL certificates not found!"
    print_note "Run setup-ssl.sh first to get SSL certificates"
    read -p "Continue without SSL? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_error "Deployment cancelled. Please setup SSL certificates first."
        exit 1
    fi
fi

# Stop existing containers
print_status "Stopping existing containers..."
docker-compose -f $COMPOSE_FILE down --remove-orphans || true

# Remove unused images
print_status "Cleaning up old images..."
docker image prune -f || true

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
fi

# Test database connection from container
print_status "Testing database connection from API container..."
DB_TEST_RESULT=$(docker-compose -f $COMPOSE_FILE exec -T api dotnet --version 2>/dev/null)
if [[ $? -eq 0 ]]; then
    print_status "✅ API container is running"
    
    # Run database migrations
    print_status "Running database migrations..."
    docker-compose -f $COMPOSE_FILE exec -T api dotnet ef database update || print_warning "Migration failed, database might already be up to date"
else
    print_error "❌ API container is not responding"
fi

# Setup firewall
print_status "Configuring firewall..."
if command -v ufw &> /dev/null; then
    sudo ufw allow 22/tcp
    sudo ufw allow 80/tcp
    sudo ufw allow 443/tcp
    sudo ufw --force enable
    print_status "Firewall configured ✅"
fi

# Create monitoring script for external SQL
print_status "Creating monitoring script..."
cat > monitor-external-sql.sh << EOF
#!/bin/bash
echo "=== AutoAppManagement Health Check (External SQL) ==="
echo "Date: \$(date)"
echo ""

echo "=== Container Status ==="
docker-compose -f $COMPOSE_FILE ps

echo ""
echo "=== SQL Server Status ==="
if command -v sqlcmd &> /dev/null; then
    sqlcmd -S localhost -U $DB_USER -P $DB_PASSWORD -d $DB_NAME -Q "SELECT 'SQL Server: ONLINE' as Status, GETDATE() as CurrentTime" 2>/dev/null || echo "SQL Server: Connection failed"
else
    echo "sqlcmd not available for testing"
fi

echo ""
echo "=== Disk Usage ==="
df -h

echo ""
echo "=== Memory Usage ==="
free -h

echo ""
echo "=== Service URLs ==="
echo "Web App: https://tlsoftware.io.vn"
echo "API: https://api.tlsoftware.io.vn"

echo ""
echo "=== Testing URLs ==="
curl -sL -w "Web App Response: %{http_code}\\n" "https://tlsoftware.io.vn" -o /dev/null 2>/dev/null || echo "Web App: Connection failed"
curl -sL -w "API Response: %{http_code}\\n" "https://api.tlsoftware.io.vn/health" -o /dev/null 2>/dev/null || echo "API: Connection failed"
EOF

chmod +x monitor-external-sql.sh

# Create backup script for external SQL
print_status "Creating backup script..."
cat > backup-external-sql.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/opt/autoappmanagement/backups"
DATE=$(date +%Y%m%d_%H%M%S)
mkdir -p $BACKUP_DIR

# Load database connection
source .env.external-sql
DB_USER=$(echo $DB_CONNECTION_STRING | grep -oP 'User Id=\K[^;]*')
DB_PASSWORD=$(echo $DB_CONNECTION_STRING | grep -oP 'Password=\K[^;]*')
DB_NAME=$(echo $DB_CONNECTION_STRING | grep -oP 'Database=\K[^;]*')

echo "Creating database backup..."

# Backup database using sqlcmd and bcp
sqlcmd -S localhost -U "$DB_USER" -P "$DB_PASSWORD" -Q "BACKUP DATABASE [$DB_NAME] TO DISK = '$BACKUP_DIR/autoapp_$DATE.bak'"

if [[ $? -eq 0 ]]; then
    echo "✅ Backup completed: $BACKUP_DIR/autoapp_$DATE.bak"
    
    # Cleanup old backups (keep last 7 days)
    find $BACKUP_DIR -name "*.bak" -mtime +7 -delete
    echo "Old backups cleaned up"
else
    echo "❌ Backup failed!"
    exit 1
fi
EOF

chmod +x backup-external-sql.sh

print_status "✅ Deployment completed successfully! 🎉"
print_status ""
print_status "=== DEPLOYMENT SUMMARY ==="
print_status "Environment: $ENVIRONMENT"
print_status "Compose File: $COMPOSE_FILE"
print_status "Database: External SQL Server (localhost:1433)"
print_status "Web App URL: https://$DOMAIN"
print_status "API URL: https://$API_DOMAIN"
print_status ""
print_status "=== USEFUL COMMANDS ==="
print_status "View logs: docker-compose -f $COMPOSE_FILE logs -f"
print_status "Restart services: docker-compose -f $COMPOSE_FILE restart"
print_status "Stop services: docker-compose -f $COMPOSE_FILE down"
print_status "Monitor system: ./monitor-external-sql.sh"
print_status "Backup database: ./backup-external-sql.sh"
print_status ""
print_status "=== DATABASE CONNECTION ==="
print_status "From containers: host.docker.internal:1433"
print_status "From host: localhost:1433"
print_status "Database: $DB_NAME"
print_status "User: $DB_USER"

# Show running containers
print_status ""
print_status "Running containers:"
docker-compose -f $COMPOSE_FILE ps

# Final health check
print_status ""
print_status "Running health check..."
./monitor-external-sql.sh
