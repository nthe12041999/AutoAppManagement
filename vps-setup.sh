#!/bin/bash

# Quick setup script cho VPS mới
# Chạy script này đầu tiên trên VPS Ubuntu/CentOS mới

echo "🚀 AutoAppManagement VPS Quick Setup for tlsoftware.io.vn"

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

# Check if running as root
if [[ $EUID -eq 0 ]]; then
   print_error "Please run this script as a regular user with sudo privileges, not as root"
   exit 1
fi

print_status "Starting VPS setup for AutoAppManagement..."

# Detect OS
if [[ -f /etc/os-release ]]; then
    . /etc/os-release
    OS=$NAME
    VER=$VERSION_ID
else
    print_error "Cannot detect OS version"
    exit 1
fi

print_status "Detected OS: $OS $VER"

# Update system
print_status "Updating system packages..."
if [[ $OS == *"Ubuntu"* ]] || [[ $OS == *"Debian"* ]]; then
    sudo apt update && sudo apt upgrade -y
    sudo apt install -y curl wget git unzip htop nano vim ufw
elif [[ $OS == *"CentOS"* ]] || [[ $OS == *"Red Hat"* ]]; then
    sudo yum update -y
    sudo yum install -y curl wget git unzip htop nano vim firewalld
else
    print_warning "Unsupported OS, continuing anyway..."
fi

# Install Docker
print_status "Installing Docker..."
if ! command -v docker &> /dev/null; then
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    sudo usermod -aG docker $USER
    rm get-docker.sh
    print_status "Docker installed successfully!"
else
    print_status "Docker already installed"
fi

# Install Docker Compose
print_status "Installing Docker Compose..."
if ! command -v docker-compose &> /dev/null; then
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
    print_status "Docker Compose installed successfully!"
else
    print_status "Docker Compose already installed"
fi

# Create application directory
print_status "Creating application directory..."
sudo mkdir -p /opt/autoappmanagement
sudo chown $USER:$USER /opt/autoappmanagement

# Setup basic firewall
print_status "Configuring firewall..."
if [[ $OS == *"Ubuntu"* ]] || [[ $OS == *"Debian"* ]]; then
    sudo ufw --force reset
    sudo ufw default deny incoming
    sudo ufw default allow outgoing
    sudo ufw allow 22/tcp comment 'SSH'
    sudo ufw allow 80/tcp comment 'HTTP'
    sudo ufw allow 443/tcp comment 'HTTPS'
    sudo ufw --force enable
elif [[ $OS == *"CentOS"* ]] || [[ $OS == *"Red Hat"* ]]; then
    sudo systemctl enable firewalld
    sudo systemctl start firewalld
    sudo firewall-cmd --permanent --add-service=ssh
    sudo firewall-cmd --permanent --add-service=http
    sudo firewall-cmd --permanent --add-service=https
    sudo firewall-cmd --reload
fi

# Create swap if needed (for low memory VPS)
MEMORY=$(free -m | awk 'NR==2{printf "%.0f", $2}')
if [[ $MEMORY -lt 2048 ]]; then
    print_status "Creating swap file (detected ${MEMORY}MB RAM)..."
    if [[ ! -f /swapfile ]]; then
        sudo fallocate -l 2G /swapfile
        sudo chmod 600 /swapfile
        sudo mkswap /swapfile
        sudo swapon /swapfile
        echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab
        print_status "2GB swap file created"
    else
        print_status "Swap file already exists"
    fi
fi

# Install fail2ban for security
print_status "Installing fail2ban for security..."
if [[ $OS == *"Ubuntu"* ]] || [[ $OS == *"Debian"* ]]; then
    sudo apt install -y fail2ban
elif [[ $OS == *"CentOS"* ]] || [[ $OS == *"Red Hat"* ]]; then
    sudo yum install -y epel-release
    sudo yum install -y fail2ban
fi

if command -v fail2ban-server &> /dev/null; then
    sudo systemctl enable fail2ban
    sudo systemctl start fail2ban
    print_status "Fail2ban installed and started"
fi

# Setup timezone
print_status "Setting timezone to Asia/Ho_Chi_Minh..."
sudo timedatectl set-timezone Asia/Ho_Chi_Minh

# Download deployment files
print_status "Downloading deployment files..."
cd /opt/autoappmanagement

# If git repo is available
if [[ -n "${GIT_REPO}" ]]; then
    git clone $GIT_REPO .
else
    print_note "To download your application code, you can:"
    print_note "1. Use git clone: git clone <your-repo-url> ."
    print_note "2. Upload files via SCP"
    print_note "3. Download from URL"
fi

# Create basic directory structure
mkdir -p nginx/ssl
mkdir -p Database/Scripts
mkdir -p logs
mkdir -p backups

# Set proper permissions
print_status "Setting permissions..."
sudo chown -R $USER:$USER /opt/autoappmanagement

# Install useful tools
print_status "Installing additional tools..."
if [[ $OS == *"Ubuntu"* ]] || [[ $OS == *"Debian"* ]]; then
    sudo apt install -y tree ncdu iotop
elif [[ $OS == *"CentOS"* ]] || [[ $OS == *"Red Hat"* ]]; then
    sudo yum install -y tree ncdu iotop
fi

# Create .bashrc aliases
print_status "Creating useful aliases..."
cat >> ~/.bashrc << 'EOF'

# AutoAppManagement aliases
alias app='cd /opt/autoappmanagement'
alias app-logs='cd /opt/autoappmanagement && docker-compose -f docker-compose.prod.yml logs'
alias app-ps='cd /opt/autoappmanagement && docker-compose -f docker-compose.prod.yml ps'
alias app-restart='cd /opt/autoappmanagement && docker-compose -f docker-compose.prod.yml restart'
alias app-monitor='cd /opt/autoappmanagement && ./monitor.sh'
alias app-backup='cd /opt/autoappmanagement && ./backup.sh'
EOF

print_status "✅ VPS setup completed successfully!"
print_status ""
print_status "=== SYSTEM INFORMATION ==="
print_status "OS: $OS $VER"
print_status "Memory: ${MEMORY}MB"
print_status "Docker: $(docker --version 2>/dev/null || echo 'Not available')"
print_status "Docker Compose: $(docker-compose --version 2>/dev/null || echo 'Not available')"
print_status ""
print_status "=== NEXT STEPS ==="
print_note "1. Logout and login again (or run: newgrp docker)"
print_note "2. Upload your application code to /opt/autoappmanagement"
print_note "3. Make sure DNS points tlsoftware.io.vn to this server IP"
print_note "4. Run: cd /opt/autoappmanagement && ./setup-ssl.sh"
print_note "5. Run: ./deploy-vps.sh prod"
print_status ""
print_status "=== USEFUL COMMANDS ==="
print_status "Go to app directory: app"
print_status "View logs: app-logs"
print_status "Check containers: app-ps"
print_status "Restart services: app-restart"
print_status "Monitor system: app-monitor"
print_status ""
print_warning "Please logout and login again to apply Docker group permissions!"

# Show current IP
IP=$(curl -s ifconfig.me || curl -s icanhazip.com || echo "Unable to detect")
print_status "Server IP: $IP"
print_note "Make sure tlsoftware.io.vn points to: $IP"
