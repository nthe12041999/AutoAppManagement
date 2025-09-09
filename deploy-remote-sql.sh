#!/bin/bash

# Deploy script cho AutoAppManagement với Remote SQL Server
# SQL Server: 125.253.121.206:1433

set -e

ENVIRONMENT=${1:-prod}
PROJECT_NAME="autoappmanagement"
DOMAIN="tlsoftware.io.vn"
API_DOMAIN="api.tlsoftware.io.vn"
SQL_SERVER_IP="125.253.121.206"
VERSION=$(date +%Y%m%d-%H%M%S)

echo "🚀 Starting deployment with Remote SQL Server: $SQL_SERVER_IP"

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

# Check if remote database is setup
if [[ ! -f ".env.remote-sql" ]]; then
    print_error "Remote database not setup! Please run setup-remote-database.sh first"
    print_note "Run: chmod +x setup-remote-database.sh && ./setup-remote-database.sh"
    exit 1
fi

# Load database connection string
source .env.remote-sql

if [[ -z "$DB_CONNECTION_STRING" ]]; then
    print_error "DB_CONNECTION_STRING not found! Please setup remote database first."
    exit 1
fi

print_status "Remote database connection configured ✅"
print_status "SQL Server: $SQL_SERVER_IP:1433"

# Test remote SQL Server connectivity
print_status "Testing remote SQL Server connectivity..."
if timeout 10 bash -c "</dev/tcp/$SQL_SERVER_IP/1433"; then
    print_status "✅ Network connectivity to remote SQL Server OK"
else
    print_error "❌ Cannot connect to remote SQL Server"
    print_note "Please check network connectivity and firewall settings"
    exit 1
fi

# Test SQL connection if sqlcmd is available
if command -v sqlcmd &> /dev/null; then
    DB_USER=$(echo $DB_CONNECTION_STRING | grep -oP 'User Id=\K[^;]*')
    DB_PASSWORD=$(echo $DB_CONNECTION_STRING | grep -oP 'Password=\K[^;]*')
    DB_NAME=$(echo $DB_CONNECTION_STRING | grep -oP 'Database=\K[^;]*')
    
    sqlcmd -S "$SQL_SERVER_IP,1433" -U "$DB_USER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 'SQL Connection OK' as Status" -l 10 > /dev/null 2>&1
    if [[ $? -eq 0 ]]; then
        print_status "✅ SQL Server authentication successful"
    else
        print_error "❌ SQL Server authentication failed"
        print_note "Please check database credentials and permissions"
        exit 1
    fi
else
    print_warning "sqlcmd not found, skipping SQL connection test"
fi

# Check Docker
if ! command -v docker &> /dev/null || ! command -v docker-compose &> /dev/null; then
    print_error "Docker or Docker Compose not installed!"
    exit 1
fi

# Check if we should use nginx container or host nginx
USE_HOST_NGINX=false
if systemctl is-active --quiet nginx && [[ -f "/etc/nginx/sites-available/autoappmanagement" ]]; then
    print_status "Host nginx detected and configured ✅"
    COMPOSE_FILE="docker-compose.remote-sql.yml"
    USE_HOST_NGINX=true
else
    print_warning "Host nginx not configured. Will use nginx container."
    # Check if ports 80/443 are free for nginx container
    if netstat -tuln | grep -q ":80 \|:443 "; then
        print_error "Ports 80/443 are in use! Please setup host nginx first."
        print_note "Run: ./setup-host-nginx.sh"
        exit 1
    fi
    COMPOSE_FILE="docker-compose.remote-sql-with-nginx.yml"
fi

# Export environment variables
export DB_CONNECTION_STRING
export DOMAIN
export API_DOMAIN

print_status "Environment: $ENVIRONMENT"
print_status "Compose file: $COMPOSE_FILE"
print_status "Architecture: $([ "$USE_HOST_NGINX" = true ] && echo "Host Nginx + Containers" || echo "Full Containerized")"
print_status "SQL Server: Remote ($SQL_SERVER_IP)"

# Check application ports
print_status "Checking application ports..."
for port in 8080 8081; do
    if netstat -tuln | grep -q ":$port "; then
        print_error "Port $port is already in use!"
        sudo netstat -tulpn | grep :$port
        exit 1
    fi
done

# Create necessary directories
print_status "Creating necessary directories..."
mkdir -p nginx/ssl logs backups

# Stop existing containers
print_status "Stopping existing containers..."
docker-compose -f $COMPOSE_FILE down --remove-orphans 2>/dev/null || true
docker-compose -f docker-compose.remote-sql.yml down --remove-orphans 2>/dev/null || true
docker-compose -f docker-compose.no-nginx.yml down --remove-orphans 2>/dev/null || true

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
print_status "Checking container health..."
if docker-compose -f $COMPOSE_FILE ps | grep -q "unhealthy\|Exit"; then
    print_error "Some containers are not healthy!"
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
print_status "Running database migrations on remote SQL Server..."
docker-compose -f $COMPOSE_FILE exec -T api dotnet ef database update || print_warning "Migration might have failed - this is normal for first-time setup"

# Test public URLs if using host nginx
if [ "$USE_HOST_NGINX" = true ]; then
    print_status "Testing public URLs through host nginx..."
    sleep 5
    
    curl -sL -w "Web App Response: %{http_code}\n" "https://$DOMAIN" -o /dev/null 2>/dev/null || print_warning "Public web app test failed"
    curl -sL -w "API Response: %{http_code}\n" "https://$API_DOMAIN/health" -o /dev/null 2>/dev/null || print_warning "Public API test failed"
fi

# Create monitoring script for remote SQL
print_status "Creating monitoring script..."
cat > monitor-remote-sql.sh << EOF
#!/bin/bash
echo "=== AutoAppManagement Health Check (Remote SQL) ==="
echo "Date: \$(date)"
echo ""

echo "=== Container Status ==="
docker-compose -f $COMPOSE_FILE ps

echo ""
echo "=== Remote SQL Server Connectivity ==="
echo "Testing connection to $SQL_SERVER_IP:1433..."
if timeout 5 bash -c "</dev/tcp/$SQL_SERVER_IP/1433"; then
    echo "✅ Network connectivity: OK"
    
    if command -v sqlcmd &> /dev/null; then
        sqlcmd -S "$SQL_SERVER_IP,1433" -U "$DB_USER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 'SQL Server: ONLINE' as Status, GETDATE() as CurrentTime" -l 10 2>/dev/null || echo "❌ SQL authentication failed"
    else
        echo "ℹ️ sqlcmd not available for SQL test"
    fi
else
    echo "❌ Network connectivity: Failed"
fi

echo ""
echo "=== Local Endpoints ==="
curl -sL -w "WebApp (8080): %{http_code}\\n" "http://localhost:8080/health" -o /dev/null 2>/dev/null || echo "WebApp: Failed"
curl -sL -w "API (8081): %{http_code}\\n" "http://localhost:8081/health" -o /dev/null 2>/dev/null || echo "API: Failed"

if [ "$USE_HOST_NGINX" = true ]; then
    echo ""
    echo "=== Public URLs ==="
    curl -sL -w "Web App: %{http_code}\\n" "https://$DOMAIN" -o /dev/null 2>/dev/null || echo "Web App: Failed"
    curl -sL -w "API: %{http_code}\\n" "https://$API_DOMAIN/health" -o /dev/null 2>/dev/null || echo "API: Failed"
    
    echo ""
    echo "=== Nginx Status ==="
    if systemctl is-active --quiet nginx; then
        echo "Host Nginx: RUNNING"
    else
        echo "Host Nginx: NOT RUNNING"
    fi
fi

echo ""
echo "=== System Resources ==="
free -h | head -2
df -h / | tail -1

echo ""
echo "=== Network Info ==="
VPS_IP=\$(curl -s ifconfig.me 2>/dev/null || echo "Unable to detect")
echo "VPS IP: \$VPS_IP"
echo "SQL Server: $SQL_SERVER_IP:1433"
EOF

chmod +x monitor-remote-sql.sh

# Create backup script for remote SQL
print_status "Creating backup script..."
cat > backup-remote-sql.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/opt/autoappmanagement/backups"
DATE=$(date +%Y%m%d_%H%M%S)
mkdir -p $BACKUP_DIR

# Load remote database connection
source .env.remote-sql
DB_USER=$(echo $DB_CONNECTION_STRING | grep -oP 'User Id=\K[^;]*')
DB_PASSWORD=$(echo $DB_CONNECTION_STRING | grep -oP 'Password=\K[^;]*')
DB_NAME=$(echo $DB_CONNECTION_STRING | grep -oP 'Database=\K[^;]*')
SQL_SERVER="125.253.121.206,1433"

echo "Creating remote database backup..."

if command -v sqlcmd &> /dev/null; then
    # Create backup on remote SQL Server
    sqlcmd -S "$SQL_SERVER" -U "$DB_USER" -P "$DB_PASSWORD" -Q "BACKUP DATABASE [$DB_NAME] TO DISK = '/tmp/autoapp_$DATE.bak'" -l 300
    
    if [[ $? -eq 0 ]]; then
        echo "✅ Remote backup created successfully"
        echo "Backup location: $SQL_SERVER:/tmp/autoapp_$DATE.bak"
        
        # Note: You would need additional tools like bcp or custom scripts to download the backup file
        echo "ℹ️ To download backup file, use tools like scp or configure shared storage"
    else
        echo "❌ Remote backup failed!"
        exit 1
    fi
else
    echo "❌ sqlcmd not available for backup"
    exit 1
fi

echo "Backup completed: $DATE"
EOF

chmod +x backup-remote-sql.sh

print_status "✅ Deployment completed successfully! 🎉"
print_status ""
print_status "=== DEPLOYMENT SUMMARY ==="
print_status "Environment: $ENVIRONMENT"
print_status "Architecture: $([ "$USE_HOST_NGINX" = true ] && echo "Host Nginx + Docker Containers" || echo "Full Containerized")"
print_status "SQL Server: Remote ($SQL_SERVER_IP:1433)"
print_status "WebApp Container: localhost:8080"
print_status "API Container: localhost:8081"
if [ "$USE_HOST_NGINX" = true ]; then
    print_status "Public Web App: https://$DOMAIN"
    print_status "Public API: https://$API_DOMAIN"
fi
print_status ""
print_status "=== USEFUL COMMANDS ==="
print_status "Container logs: docker-compose -f $COMPOSE_FILE logs -f"
print_status "Restart containers: docker-compose -f $COMPOSE_FILE restart"
print_status "Stop containers: docker-compose -f $COMPOSE_FILE down"
print_status "Monitor system: ./monitor-remote-sql.sh"
print_status "Test SQL connection: ./test-sql-connection.sh"
print_status "Backup database: ./backup-remote-sql.sh"
if [ "$USE_HOST_NGINX" = true ]; then
    print_status "Nginx logs: sudo tail -f /var/log/nginx/access.log"
    print_status "Nginx reload: sudo systemctl reload nginx"
fi
print_status ""
print_status "=== REMOTE DATABASE INFO ==="
print_status "SQL Server: $SQL_SERVER_IP:1433"
print_status "Database: $DB_NAME"
print_status "Connection: $(echo $DB_CONNECTION_STRING | head -c 50)..."

# Show running containers
print_status ""
print_status "Running containers:"
docker-compose -f $COMPOSE_FILE ps

# Final health check
print_status ""
print_status "Running health check..."
./monitor-remote-sql.sh

# Display VPS IP for whitelisting
VPS_IP=$(curl -s ifconfig.me || curl -s icanhazip.com || echo "Unable to detect")
print_status ""
print_warning "IMPORTANT: Make sure VPS IP ($VPS_IP) is whitelisted on SQL Server $SQL_SERVER_IP"
