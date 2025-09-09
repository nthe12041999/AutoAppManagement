#!/bin/bash

# Script test connectivity tới remote SQL Server 125.253.121.206

echo "🌐 Testing connectivity to Remote SQL Server..."

SQL_SERVER_IP="125.253.121.206"
SQL_SERVER_PORT="1433"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() {
    echo -e "${GREEN}[✅]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[⚠️]${NC} $1"
}

print_error() {
    echo -e "${RED}[❌]${NC} $1"
}

print_note() {
    echo -e "${BLUE}[ℹ️]${NC} $1"
}

echo "=== NETWORK CONNECTIVITY TEST ==="
echo "Target: $SQL_SERVER_IP:$SQL_SERVER_PORT"
echo "Date: $(date)"
echo ""

# Test 1: Basic network connectivity
echo "1. Testing basic network connectivity..."
if timeout 10 bash -c "</dev/tcp/$SQL_SERVER_IP/$SQL_SERVER_PORT"; then
    print_status "Network connectivity: SUCCESS"
    NETWORK_OK=true
else
    print_error "Network connectivity: FAILED"
    NETWORK_OK=false
fi

# Test 2: Ping test
echo ""
echo "2. Testing ping..."
if ping -c 3 -W 5 $SQL_SERVER_IP > /dev/null 2>&1; then
    print_status "Ping test: SUCCESS"
else
    print_warning "Ping test: FAILED (might be blocked by firewall)"
fi

# Test 3: Port scan
echo ""
echo "3. Testing port accessibility..."
if command -v nmap &> /dev/null; then
    nmap_result=$(nmap -p $SQL_SERVER_PORT $SQL_SERVER_IP 2>/dev/null | grep $SQL_SERVER_PORT)
    if echo "$nmap_result" | grep -q "open"; then
        print_status "Port $SQL_SERVER_PORT: OPEN"
    else
        print_error "Port $SQL_SERVER_PORT: CLOSED or FILTERED"
    fi
else
    print_note "nmap not available, skipping port scan"
fi

# Test 4: SQL Server connection (if credentials available)
echo ""
echo "4. Testing SQL Server authentication..."
if [[ -f ".env.remote-sql" ]]; then
    source .env.remote-sql
    DB_USER=$(echo $DB_CONNECTION_STRING | grep -oP 'User Id=\K[^;]*' 2>/dev/null)
    DB_PASSWORD=$(echo $DB_CONNECTION_STRING | grep -oP 'Password=\K[^;]*' 2>/dev/null)
    DB_NAME=$(echo $DB_CONNECTION_STRING | grep -oP 'Database=\K[^;]*' 2>/dev/null)
    
    if [[ -n "$DB_USER" ]] && [[ -n "$DB_PASSWORD" ]] && command -v sqlcmd &> /dev/null; then
        echo "Testing with user: $DB_USER"
        if sqlcmd -S "$SQL_SERVER_IP,$SQL_SERVER_PORT" -U "$DB_USER" -P "$DB_PASSWORD" -d "$DB_NAME" -Q "SELECT 'Authentication SUCCESS' as Status, GETDATE() as CurrentTime" -l 10 > /dev/null 2>&1; then
            print_status "SQL Authentication: SUCCESS"
            AUTH_OK=true
        else
            print_error "SQL Authentication: FAILED"
            AUTH_OK=false
        fi
    else
        print_note "SQL credentials not available or sqlcmd not installed"
        AUTH_OK="unknown"
    fi
else
    print_note "Database not configured yet (.env.remote-sql not found)"
    AUTH_OK="unknown"
fi

# Test 5: DNS resolution
echo ""
echo "5. Testing DNS resolution..."
if nslookup $SQL_SERVER_IP > /dev/null 2>&1; then
    print_status "DNS resolution: SUCCESS"
else
    print_warning "DNS resolution: No reverse DNS (normal for IP addresses)"
fi

# Test 6: MTU and network path
echo ""
echo "6. Testing network path..."
if command -v traceroute &> /dev/null; then
    echo "Traceroute to $SQL_SERVER_IP (first 5 hops):"
    traceroute -m 5 $SQL_SERVER_IP 2>/dev/null | head -7
elif command -v tracepath &> /dev/null; then
    echo "Tracepath to $SQL_SERVER_IP:"
    tracepath $SQL_SERVER_IP 2>/dev/null | head -5
else
    print_note "traceroute/tracepath not available"
fi

# Summary
echo ""
echo "=== CONNECTIVITY SUMMARY ==="
if [ "$NETWORK_OK" = true ]; then
    print_status "✅ Network connectivity is working"
    
    if [ "$AUTH_OK" = true ]; then
        print_status "✅ SQL Server authentication is working"
        echo ""
        print_status "🎉 Ready to deploy! Run: ./deploy-remote-sql.sh"
    elif [ "$AUTH_OK" = false ]; then
        print_error "❌ SQL Server authentication failed"
        echo ""
        print_note "🔧 Setup database first: ./setup-remote-database.sh"
    else
        print_warning "⚠️ SQL Server authentication not tested"
        echo ""
        print_note "🔧 Setup database: ./setup-remote-database.sh"
    fi
else
    print_error "❌ Network connectivity failed"
    echo ""
    echo "=== TROUBLESHOOTING STEPS ==="
    print_note "1. Check if SQL Server is running on $SQL_SERVER_IP:$SQL_SERVER_PORT"
    print_note "2. Verify firewall settings on SQL Server"
    print_note "3. Check if SQL Server allows remote connections"
    print_note "4. Verify TCP/IP protocol is enabled on SQL Server"
    print_note "5. Check network connectivity between VPS and SQL Server"
fi

# Get current VPS IP
echo ""
echo "=== NETWORK INFORMATION ==="
VPS_IP=$(curl -s ifconfig.me || curl -s icanhazip.com || echo "Unable to detect")
print_note "Current VPS IP: $VPS_IP"
print_note "Target SQL Server: $SQL_SERVER_IP:$SQL_SERVER_PORT"
print_note "Make sure VPS IP ($VPS_IP) is whitelisted on SQL Server firewall"

# Test if we can reach the internet (for comparison)
echo ""
echo "=== INTERNET CONNECTIVITY TEST ==="
if curl -s --connect-timeout 5 google.com > /dev/null; then
    print_status "Internet connectivity: OK"
else
    print_error "Internet connectivity: Failed"
fi

echo ""
echo "Test completed at: $(date)"
