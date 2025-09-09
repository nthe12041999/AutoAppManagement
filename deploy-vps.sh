#!/bin/bash

# Deploy script for AutoAppManagement on VPS với domain tlsoftware.io.vn
# Usage: ./deploy-vps.sh [environment]
# Environment: dev, staging, prod (default: prod)

set -e

ENVIRONMENT=${1:-prod}
PROJECT_NAME="autoappmanagement"
DOMAIN="tlsoftware.io.vn"
API_DOMAIN="api.tlsoftware.io.vn"
VERSION=$(date +%Y%m%d-%H%M%S)

echo "🚀 Starting VPS deployment for domain: $DOMAIN"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Check if running on VPS
if [[ ! -f "/etc/os-release" ]]; then
    print_error "This script should be run on a Linux VPS!"
    exit 1
fi

print_status "Checking system requirements..."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    print_status "Installing Docker..."
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    sudo usermod -aG docker $USER
    rm get-docker.sh
    print_status "Docker installed successfully!"
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    print_status "Installing Docker Compose..."
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
    print_status "Docker Compose installed successfully!"
fi

# Create application directory
APP_DIR="/opt/autoappmanagement"
if [[ ! -d "$APP_DIR" ]]; then
    print_status "Creating application directory: $APP_DIR"
    sudo mkdir -p $APP_DIR
    sudo chown $USER:$USER $APP_DIR
fi

cd $APP_DIR

# Create necessary directories
print_status "Creating necessary directories..."
mkdir -p nginx/ssl
mkdir -p Database/Scripts
mkdir -p logs
mkdir -p backups

# Set environment variables based on environment
case $ENVIRONMENT in
    "dev")
        COMPOSE_FILE="docker-compose.yml"
        DB_CONNECTION_STRING="Server=sqlserver,1433;Database=AutoAppManagement_Dev;User Id=sa;Password=AutoApp@123456;TrustServerCertificate=true;Encrypt=false;"
        ;;
    "staging")
        COMPOSE_FILE="docker-compose.prod.yml"
        DB_CONNECTION_STRING="Server=your-staging-db-server;Database=AutoAppManagement_Staging;User Id=your-user;Password=your-password;TrustServerCertificate=true;Encrypt=false;"
        ;;
    "prod")
        COMPOSE_FILE="docker-compose.prod.yml"
        # Sử dụng external database hoặc local SQL Server container
        DB_CONNECTION_STRING="Server=sqlserver,1433;Database=AutoAppManagement;User Id=sa;Password=AutoApp@Production@2024;TrustServerCertificate=true;Encrypt=false;"
        ;;
    *)
        print_error "Invalid environment: $ENVIRONMENT"
        exit 1
        ;;
esac

# Export environment variables
export DB_CONNECTION_STRING
export DOMAIN
export API_DOMAIN

print_status "Environment: $ENVIRONMENT"
print_status "Using compose file: $COMPOSE_FILE"
print_status "Domain: $DOMAIN"
print_status "API Domain: $API_DOMAIN"

# Check SSL certificates
print_status "Checking SSL certificates..."
if [[ ! -f "nginx/ssl/tlsoftware.io.vn.crt" ]] || [[ ! -f "nginx/ssl/tlsoftware.io.vn.key" ]]; then
    print_warning "SSL certificates not found!"
    print_note "You need to:"
    print_note "1. Obtain SSL certificates for $DOMAIN"
    print_note "2. Place them in nginx/ssl/ directory:"
    print_note "   - nginx/ssl/tlsoftware.io.vn.crt"
    print_note "   - nginx/ssl/tlsoftware.io.vn.key"
    print_note ""
    print_note "You can use Let's Encrypt with certbot:"
    print_note "sudo apt install certbot"
    print_note "sudo certbot certonly --standalone -d $DOMAIN -d www.$DOMAIN -d $API_DOMAIN"
    print_note "Then copy the certificates to nginx/ssl/"
    read -p "Do you want to continue without SSL certificates? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_error "Deployment cancelled. Please setup SSL certificates first."
        exit 1
    fi
fi

# Stop existing containers
print_status "Stopping existing containers..."
docker-compose -f $COMPOSE_FILE down --remove-orphans || true

# Remove old images (optional)
print_status "Cleaning up old images..."
docker image prune -f || true

# Pull latest images or build
print_status "Building and starting containers..."
docker-compose -f $COMPOSE_FILE up -d --build

# Wait for services to be healthy
print_status "Waiting for services to be ready..."
sleep 60

# Check if services are running
print_status "Checking service health..."
if docker-compose -f $COMPOSE_FILE ps | grep -q "unhealthy\|Exit"; then
    print_error "Some services are not healthy!"
    print_status "Container logs:"
    docker-compose -f $COMPOSE_FILE logs
    exit 1
fi

# Setup firewall rules
print_status "Configuring firewall..."
if command -v ufw &> /dev/null; then
    sudo ufw allow 22/tcp    # SSH
    sudo ufw allow 80/tcp    # HTTP
    sudo ufw allow 443/tcp   # HTTPS
    sudo ufw --force enable
    print_status "Firewall configured successfully!"
fi

# Run database migrations (if using local SQL Server)
if [ "$ENVIRONMENT" = "prod" ]; then
    print_status "Waiting for database to be ready..."
    sleep 30
    print_status "Running database migrations..."
    docker-compose -f $COMPOSE_FILE exec -T api dotnet ef database update || print_warning "Migration failed, database might already be up to date"
fi

# Create backup script
print_status "Creating backup script..."
cat > backup.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/opt/autoappmanagement/backups"
DATE=$(date +%Y%m%d_%H%M%S)
mkdir -p $BACKUP_DIR

# Backup database
docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "AutoApp@Production@2024" -Q "BACKUP DATABASE [AutoAppManagement] TO DISK = N'/tmp/autoapp_$DATE.bak'"
docker cp autoappmanagement_sqlserver_1:/tmp/autoapp_$DATE.bak $BACKUP_DIR/

# Cleanup old backups (keep last 7 days)
find $BACKUP_DIR -name "*.bak" -mtime +7 -delete

echo "Backup completed: $BACKUP_DIR/autoapp_$DATE.bak"
EOF

chmod +x backup.sh

# Create monitoring script
print_status "Creating monitoring script..."
cat > monitor.sh << 'EOF'
#!/bin/bash
echo "=== AutoAppManagement Health Check ==="
echo "Date: $(date)"
echo ""

echo "=== Container Status ==="
docker-compose ps

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
curl -sL -w "Web App Response: %{http_code}\n" "https://tlsoftware.io.vn" -o /dev/null
curl -sL -w "API Response: %{http_code}\n" "https://api.tlsoftware.io.vn/health" -o /dev/null
EOF

chmod +x monitor.sh

print_status "Deployment completed successfully! 🎉"
print_status ""
print_status "=== DEPLOYMENT SUMMARY ==="
print_status "Environment: $ENVIRONMENT"
print_status "Application Directory: $APP_DIR"
print_status "Web App URL: https://$DOMAIN"
print_status "API URL: https://$API_DOMAIN"
print_status ""
print_status "=== USEFUL COMMANDS ==="
print_status "View logs: docker-compose -f $COMPOSE_FILE logs -f"
print_status "Restart services: docker-compose -f $COMPOSE_FILE restart"
print_status "Stop services: docker-compose -f $COMPOSE_FILE down"
print_status "Monitor system: ./monitor.sh"
print_status "Backup database: ./backup.sh"
print_status ""
print_status "=== NEXT STEPS ==="
print_note "1. Point your domain DNS to this VPS IP address"
print_note "2. Setup SSL certificates (Let's Encrypt recommended)"
print_note "3. Configure monitoring and alerting"
print_note "4. Setup automated backups (cron job)"
print_note "5. Test all functionality"

# Show running containers
print_status ""
print_status "Running containers:"
docker-compose -f $COMPOSE_FILE ps

# Final health check
print_status ""
print_status "Running health check..."
./monitor.sh
