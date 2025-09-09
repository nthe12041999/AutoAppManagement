#!/bin/bash

# Script setup database cho AutoAppManagement trên Remote SQL Server
# SQL Server IP: 125.253.121.206:1433

echo "🌐 Setting up AutoAppManagement database on Remote SQL Server..."

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

# Remote SQL Server configuration
SQL_SERVER_IP="125.253.121.206"
SQL_SERVER_PORT="1433"
DB_NAME="AutoAppManagement"
DB_USER="autoapp_user"
DB_PASSWORD="AutoApp@User@2024"
SA_USER="sa"
SA_PASSWORD=""

echo "=== REMOTE SQL SERVER CONFIGURATION ==="
echo "SQL Server: $SQL_SERVER_IP:$SQL_SERVER_PORT"
echo "Database: $DB_NAME"
echo "User: $DB_USER"
echo ""

# Get SA password
echo "Enter SQL Server SA password for $SQL_SERVER_IP:"
read -s SA_PASSWORD

if [[ -z "$SA_PASSWORD" ]]; then
    print_error "SA password is required!"
    exit 1
fi

# Test network connectivity first
print_status "Testing network connectivity to SQL Server..."
if timeout 10 bash -c "</dev/tcp/$SQL_SERVER_IP/$SQL_SERVER_PORT"; then
    print_status "✅ Network connectivity to $SQL_SERVER_IP:$SQL_SERVER_PORT OK"
else
    print_error "❌ Cannot connect to $SQL_SERVER_IP:$SQL_SERVER_PORT"
    print_note "Please check:"
    print_note "1. SQL Server is running on $SQL_SERVER_IP:$SQL_SERVER_PORT"
    print_note "2. Firewall allows connections from this VPS IP"
    print_note "3. SQL Server is configured for remote connections"
    print_note "4. TCP/IP protocol is enabled on SQL Server"
    exit 1
fi

# Install SQL Server tools if not available
if ! command -v sqlcmd &> /dev/null; then
    print_status "Installing SQL Server tools..."
    if [[ -f /etc/debian_version ]]; then
        # Ubuntu/Debian
        curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
        curl https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/prod.list | sudo tee /etc/apt/sources.list.d/mssql-release.list
        sudo apt-get update
        sudo ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev
        echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
        export PATH="$PATH:/opt/mssql-tools/bin"
    elif [[ -f /etc/redhat-release ]]; then
        # CentOS/RHEL
        sudo curl -o /etc/yum.repos.d/mssql-release.repo https://packages.microsoft.com/config/rhel/8/prod.repo
        sudo ACCEPT_EULA=Y yum install -y mssql-tools unixODBC-devel
        echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
        export PATH="$PATH:/opt/mssql-tools/bin"
    else
        print_error "Unsupported OS. Please install sqlcmd manually."
        exit 1
    fi
fi

# Test connection to remote SQL Server
print_status "Testing connection to remote SQL Server..."
sqlcmd -S "$SQL_SERVER_IP,$SQL_SERVER_PORT" -U "$SA_USER" -P "$SA_PASSWORD" -Q "SELECT @@VERSION" -l 30 > /dev/null 2>&1

if [[ $? -eq 0 ]]; then
    print_status "✅ Connected to remote SQL Server successfully!"
else
    print_error "❌ Cannot connect to remote SQL Server"
    print_note "Please verify:"
    print_note "1. SA password is correct"
    print_note "2. SQL Server allows remote connections"
    print_note "3. Mixed mode authentication is enabled"
    print_note "4. This VPS IP is whitelisted on SQL Server"
    exit 1
fi

# Create database setup SQL
print_status "Creating database setup script..."
cat > setup_remote_database.sql << EOF
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

-- Verify setup
SELECT 
    'Database: ' + DB_NAME() as DatabaseInfo,
    'Server: ' + @@SERVERNAME as ServerInfo,
    'Current Time: ' + CONVERT(varchar, GETDATE(), 120) as CurrentTime;
GO

-- Check if remote connections are working
SELECT 
    'Remote connection test successful from: ' + HOST_NAME() as ConnectionTest;
GO
EOF

# Execute SQL script on remote server
print_status "Executing database setup on remote SQL Server..."
sqlcmd -S "$SQL_SERVER_IP,$SQL_SERVER_PORT" -U "$SA_USER" -P "$SA_PASSWORD" -i setup_remote_database.sql -l 60

if [[ $? -eq 0 ]]; then
    print_status "✅ Remote database setup completed successfully!"
else
    print_error "❌ Remote database setup failed!"
    exit 1
fi

# Test application user connection
print_status "Testing application user connection to remote SQL Server..."
sqlcmd -S "$SQL_SERVER_IP,$SQL_SERVER_PORT" -U "$DB_USER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 'Remote connection test successful' as Result, GETDATE() as CurrentTime" -l 30

if [[ $? -eq 0 ]]; then
    print_status "✅ Application user remote connection test successful!"
else
    print_error "❌ Application user remote connection failed!"
    print_note "Please check database user permissions and network access"
fi

# Create connection string for remote SQL Server
CONNECTION_STRING="Server=$SQL_SERVER_IP,$SQL_SERVER_PORT;Database=$DB_NAME;User Id=$DB_USER;Password=$DB_PASSWORD;TrustServerCertificate=true;Encrypt=false;ConnectTimeout=30;CommandTimeout=30;"

print_status "✅ Remote database setup completed!"
print_status ""
print_status "=== REMOTE DATABASE INFORMATION ==="
print_status "SQL Server: $SQL_SERVER_IP:$SQL_SERVER_PORT"
print_status "Database Name: $DB_NAME"
print_status "Database User: $DB_USER"
print_status "Database Password: $DB_PASSWORD"
print_status ""
print_status "=== CONNECTION STRING ==="
print_status "For docker containers:"
echo "$CONNECTION_STRING"
print_status ""
print_status "=== NETWORK INFORMATION ==="
print_status "VPS → SQL Server connection: OK"
print_status "Make sure SQL Server firewall allows connections from this VPS"

# Save connection string to file
echo "DB_CONNECTION_STRING=\"$CONNECTION_STRING\"" > .env.remote-sql
print_status "Connection string saved to: .env.remote-sql"

# Create network test script
cat > test-sql-connection.sh << EOF
#!/bin/bash
echo "🌐 Testing connection to remote SQL Server..."

# Test network connectivity
echo "Testing network connectivity:"
if timeout 10 bash -c "</dev/tcp/$SQL_SERVER_IP/$SQL_SERVER_PORT"; then
    echo "✅ Network: OK"
else
    echo "❌ Network: Failed"
    exit 1
fi

# Test SQL connection
echo "Testing SQL connection:"
sqlcmd -S "$SQL_SERVER_IP,$SQL_SERVER_PORT" -U "$DB_USER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 'Connection OK' as Status, GETDATE() as Time" -l 10

echo ""
echo "Connection string:"
echo "$CONNECTION_STRING"
EOF

chmod +x test-sql-connection.sh

# Cleanup
rm setup_remote_database.sql

print_status ""
print_status "=== NEXT STEPS ==="
print_note "1. Test connection anytime: ./test-sql-connection.sh"
print_note "2. Deploy application: ./deploy-remote-sql.sh"
print_note "3. Monitor connectivity: watch ./test-sql-connection.sh"

print_status ""
print_warning "IMPORTANT SECURITY NOTES:"
print_warning "1. Ensure SQL Server firewall only allows trusted IPs"
print_warning "2. Use strong passwords for database users"
print_warning "3. Consider using SSL encryption for SQL connections"
print_warning "4. Regularly backup the remote database"
print_warning "5. Monitor SQL Server logs for suspicious activities"

print_status ""
print_status "=== VPS IP INFORMATION ==="
VPS_IP=$(curl -s ifconfig.me || curl -s icanhazip.com || echo "Unable to detect")
print_status "Current VPS IP: $VPS_IP"
print_note "Make sure this IP is whitelisted on SQL Server: $SQL_SERVER_IP"
