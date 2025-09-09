#!/bin/bash

# Deploy script for AutoAppManagement on VPS
# Usage: ./deploy.sh [environment]
# Environment: dev, staging, prod (default: prod)

set -e

ENVIRONMENT=${1:-prod}
PROJECT_NAME="autoappmanagement"
DOCKER_REGISTRY="your-registry.com"
VERSION=$(date +%Y%m%d-%H%M%S)

echo "🚀 Starting deployment for environment: $ENVIRONMENT"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
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

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed!"
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed!"
    exit 1
fi

# Create directories if they don't exist
print_status "Creating necessary directories..."
mkdir -p nginx/ssl
mkdir -p Database/Scripts
mkdir -p logs

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
        DB_CONNECTION_STRING="Server=your-prod-db-server;Database=AutoAppManagement;User Id=your-user;Password=your-password;TrustServerCertificate=true;Encrypt=false;"
        ;;
    *)
        print_error "Invalid environment: $ENVIRONMENT"
        exit 1
        ;;
esac

# Export environment variables
export DB_CONNECTION_STRING

print_status "Using compose file: $COMPOSE_FILE"
print_status "Database connection configured for $ENVIRONMENT"

# Stop existing containers
print_status "Stopping existing containers..."
docker-compose -f $COMPOSE_FILE down --remove-orphans || true

# Remove old images (optional, comment out if you want to keep them)
print_status "Cleaning up old images..."
docker image prune -f || true

# Build and start containers
print_status "Building and starting containers..."
docker-compose -f $COMPOSE_FILE up -d --build

# Wait for services to be healthy
print_status "Waiting for services to be ready..."
sleep 30

# Check if services are running
print_status "Checking service health..."
if docker-compose -f $COMPOSE_FILE ps | grep -q "unhealthy\|Exit"; then
    print_error "Some services are not healthy!"
    docker-compose -f $COMPOSE_FILE logs
    exit 1
fi

# Run database migrations (if using local SQL Server)
if [ "$ENVIRONMENT" = "dev" ]; then
    print_status "Running database migrations..."
    # Wait for SQL Server to be ready
    sleep 10
    docker-compose -f $COMPOSE_FILE exec api dotnet ef database update || print_warning "Migration failed, database might already be up to date"
fi

print_status "Deployment completed successfully! 🎉"
print_status "Web App: http://localhost:8080"
print_status "API: http://localhost:8081"

# Show running containers
print_status "Running containers:"
docker-compose -f $COMPOSE_FILE ps
