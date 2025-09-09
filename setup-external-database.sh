#!/bin/bash

# Script setup database cho AutoAppManagement trên SQL Server có sẵn
# Chạy script này để tạo database và user cho ứng dụng

echo "🗄️ Setting up AutoAppManagement database on existing SQL Server..."

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

# Database configuration
DB_NAME="AutoAppManagement"
DB_USER="autoapp_user"
DB_PASSWORD="AutoApp@User@2024"
SA_PASSWORD=""

# Get SA password
echo "Enter SQL Server SA password:"
read -s SA_PASSWORD

if [[ -z "$SA_PASSWORD" ]]; then
    print_error "SA password is required!"
    exit 1
fi

# Test connection to SQL Server
print_status "Testing connection to SQL Server..."
if command -v sqlcmd &> /dev/null; then
    sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT @@VERSION" > /dev/null 2>&1
    if [[ $? -eq 0 ]]; then
        print_status "✅ Connected to SQL Server successfully!"
    else
        print_error "❌ Cannot connect to SQL Server. Please check SA password."
        exit 1
    fi
else
    print_warning "sqlcmd not found. Installing SQL Server tools..."
    # Install SQL Server tools
    if [[ -f /etc/debian_version ]]; then
        # Ubuntu/Debian
        curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
        curl https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/mssql-tools.list | sudo tee /etc/apt/sources.list.d/mssql-tools.list
        sudo apt-get update
        sudo ACCEPT_EULA=Y apt-get install -y mssql-tools
        echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
        source ~/.bashrc
    elif [[ -f /etc/redhat-release ]]; then
        # CentOS/RHEL
        sudo curl -o /etc/yum.repos.d/mssql-tools.repo https://packages.microsoft.com/config/rhel/8/mssql-tools.repo
        sudo ACCEPT_EULA=Y yum install -y mssql-tools
        echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
        source ~/.bashrc
    else
        print_error "Unsupported OS. Please install sqlcmd manually."
        exit 1
    fi
fi

# Create database setup SQL
print_status "Creating database setup script..."
cat > setup_database.sql << EOF
-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '$DB_NAME')
BEGIN
    CREATE DATABASE [$DB_NAME];
    PRINT 'Database $DB_NAME created successfully.';
END
ELSE
BEGIN
    PRINT 'Database $DB_NAME already exists.';
END
GO

USE [$DB_NAME];
GO

-- Create application user if not exists
IF NOT EXISTS (SELECT name FROM sys.sql_logins WHERE name = '$DB_USER')
BEGIN
    CREATE LOGIN [$DB_USER] WITH PASSWORD = '$DB_PASSWORD';
    PRINT 'Login $DB_USER created successfully.';
END
ELSE
BEGIN
    PRINT 'Login $DB_USER already exists.';
END
GO

-- Create database user if not exists
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = '$DB_USER')
BEGIN
    CREATE USER [$DB_USER] FOR LOGIN [$DB_USER];
    PRINT 'User $DB_USER created successfully.';
END
ELSE
BEGIN
    PRINT 'User $DB_USER already exists.';
END
GO

-- Grant permissions
ALTER ROLE [db_owner] ADD MEMBER [$DB_USER];
PRINT 'Permissions granted to $DB_USER.';
GO

-- Enable TCP/IP and mixed mode authentication (informational)
PRINT 'Make sure SQL Server is configured for:';
PRINT '1. TCP/IP protocol enabled';
PRINT '2. Mixed mode authentication';
PRINT '3. Port 1433 accessible';
GO

-- Test connection
SELECT 
    'Database: ' + DB_NAME() as DatabaseInfo,
    'Server: ' + @@SERVERNAME as ServerInfo,
    'Version: ' + @@VERSION as VersionInfo;
GO
EOF

# Execute SQL script
print_status "Executing database setup..."
sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i setup_database.sql

if [[ $? -eq 0 ]]; then
    print_status "✅ Database setup completed successfully!"
else
    print_error "❌ Database setup failed!"
    exit 1
fi

# Test application user connection
print_status "Testing application user connection..."
sqlcmd -S localhost -U "$DB_USER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 'Connection test successful' as Result"

if [[ $? -eq 0 ]]; then
    print_status "✅ Application user connection test successful!"
else
    print_error "❌ Application user connection failed!"
fi

# Create connection string
CONNECTION_STRING="Server=host.docker.internal,1433;Database=$DB_NAME;User Id=$DB_USER;Password=$DB_PASSWORD;TrustServerCertificate=true;Encrypt=false;"

print_status "✅ Database setup completed!"
print_status ""
print_status "=== DATABASE INFORMATION ==="
print_status "Database Name: $DB_NAME"
print_status "Database User: $DB_USER"
print_status "Database Password: $DB_PASSWORD"
print_status ""
print_status "=== CONNECTION STRING ==="
print_status "For docker containers:"
echo "$CONNECTION_STRING"
print_status ""
print_status "=== NEXT STEPS ==="
print_note "1. Export the connection string:"
echo "export DB_CONNECTION_STRING=\"$CONNECTION_STRING\""
print_note "2. Run deployment:"
echo "./deploy-external-sql.sh"

# Save connection string to file
echo "DB_CONNECTION_STRING=\"$CONNECTION_STRING\"" > .env.external-sql
print_status "Connection string saved to: .env.external-sql"

# Cleanup
rm setup_database.sql

print_status ""
print_warning "IMPORTANT SECURITY NOTES:"
print_warning "1. Change default passwords in production"
print_warning "2. Use Windows Authentication if possible"
print_warning "3. Restrict network access to SQL Server"
print_warning "4. Enable SQL Server encryption"
