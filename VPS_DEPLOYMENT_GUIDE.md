# Hướng dẫn Deploy AutoAppManagement lên VPS với domain tlsoftware.io.vn

## Tổng quan
Hướng dẫn này sẽ giúp bạn deploy ứng dụng AutoAppManagement lên VPS Ubuntu/CentOS với domain `tlsoftware.io.vn`.

## Yêu cầu hệ thống
- VPS Ubuntu 20.04+ hoặc CentOS 8+
- RAM: tối thiểu 2GB (khuyến nghị 4GB+)
- Disk: tối thiểu 20GB
- Domain đã trỏ về IP của VPS
- Ports 80, 443, 22 mở

## Bước 1: Chuẩn bị VPS

### 1.1 Cập nhật hệ thống
```bash
# Ubuntu/Debian
sudo apt update && sudo apt upgrade -y

# CentOS/RHEL
sudo yum update -y
```

### 1.2 Cài đặt các gói cần thiết
```bash
# Ubuntu/Debian
sudo apt install -y curl wget git unzip

# CentOS/RHEL
sudo yum install -y curl wget git unzip
```

### 1.3 Tạo user cho ứng dụng (optional)
```bash
sudo adduser autoapp
sudo usermod -aG sudo autoapp
su - autoapp
```

## Bước 2: Cấu hình DNS

Đảm bảo các domain sau đã trỏ về IP của VPS:
- `tlsoftware.io.vn` (A record)
- `www.tlsoftware.io.vn` (CNAME hoặc A record)
- `api.tlsoftware.io.vn` (A record)

Kiểm tra DNS:
```bash
nslookup tlsoftware.io.vn
nslookup api.tlsoftware.io.vn
```

## Bước 3: Upload code lên VPS

### 3.1 Sử dụng Git (khuyến nghị)
```bash
cd /opt
sudo mkdir autoappmanagement
sudo chown $USER:$USER autoappmanagement
cd autoappmanagement
git clone https://github.com/nthe12041999/AutoAppManagement.git .
```

### 3.2 Hoặc upload qua SCP/FTP
```bash
# Từ máy local
scp -r "d:\MMO Project\AutoAppManagement\*" user@your-vps-ip:/opt/autoappmanagement/
```

## Bước 4: Setup SSL Certificates

```bash
cd /opt/autoappmanagement
chmod +x setup-ssl.sh
./setup-ssl.sh
```

Script này sẽ:
- Cài đặt certbot
- Xin SSL certificate từ Let's Encrypt
- Copy certificates vào thư mục nginx/ssl
- Setup auto-renewal

## Bước 5: Deploy ứng dụng

```bash
cd /opt/autoappmanagement
chmod +x deploy-vps.sh
./deploy-vps.sh prod
```

Script sẽ:
- Cài đặt Docker và Docker Compose
- Build và start tất cả containers
- Chạy database migrations
- Cấu hình firewall
- Tạo scripts backup và monitoring

## Bước 6: Kiểm tra deployment

### 6.1 Kiểm tra containers
```bash
cd /opt/autoappmanagement
docker-compose -f docker-compose.prod.yml ps
```

### 6.2 Kiểm tra logs
```bash
# Tất cả logs
docker-compose -f docker-compose.prod.yml logs

# Logs của webapp
docker-compose -f docker-compose.prod.yml logs webapp

# Logs của API
docker-compose -f docker-compose.prod.yml logs api

# Logs realtime
docker-compose -f docker-compose.prod.yml logs -f
```

### 6.3 Kiểm tra health
```bash
./monitor.sh
```

### 6.4 Test URLs
```bash
# Test Web App
curl -I https://tlsoftware.io.vn

# Test API
curl -I https://api.tlsoftware.io.vn/health
```

## Bước 7: Cấu hình bảo mật bổ sung

### 7.1 Firewall
```bash
# Ubuntu UFW
sudo ufw enable
sudo ufw allow 22
sudo ufw allow 80
sudo ufw allow 443

# CentOS Firewalld
sudo systemctl enable firewalld
sudo systemctl start firewalld
sudo firewall-cmd --permanent --add-service=ssh
sudo firewall-cmd --permanent --add-service=http
sudo firewall-cmd --permanent --add-service=https
sudo firewall-cmd --reload
```

### 7.2 Fail2ban (optional)
```bash
# Ubuntu
sudo apt install -y fail2ban

# CentOS
sudo yum install -y fail2ban

sudo systemctl enable fail2ban
sudo systemctl start fail2ban
```

## Bước 8: Backup và Monitoring

### 8.1 Setup automated backup
```bash
# Tạo cron job cho backup hàng ngày
crontab -e

# Thêm dòng sau (backup lúc 2:00 AM mỗi ngày)
0 2 * * * /opt/autoappmanagement/backup.sh >> /var/log/autoapp-backup.log 2>&1
```

### 8.2 Monitoring
```bash
# Tạo cron job check health mỗi 5 phút
*/5 * * * * /opt/autoappmanagement/monitor.sh >> /var/log/autoapp-monitor.log 2>&1
```

## Troubleshooting

### Vấn đề thường gặp:

#### 1. Container không start được
```bash
# Kiểm tra logs
docker-compose -f docker-compose.prod.yml logs

# Kiểm tra disk space
df -h

# Kiểm tra memory
free -h
```

#### 2. SSL Certificate lỗi
```bash
# Kiểm tra certificate
openssl x509 -in nginx/ssl/tlsoftware.io.vn.crt -text -noout

# Renew manual
sudo /usr/local/bin/renew-autoapp-ssl.sh
```

#### 3. Database connection lỗi
```bash
# Kiểm tra SQL Server container
docker exec -it autoapp_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "AutoApp@Production@2024"

# Reset database
docker-compose -f docker-compose.prod.yml down
docker volume rm autoappmanagement_sqlserver_data
docker-compose -f docker-compose.prod.yml up -d
```

#### 4. Domain không accessible
- Kiểm tra DNS settings
- Kiểm tra firewall
- Kiểm tra nginx configuration

## Commands hữu ích

```bash
# Restart tất cả services
docker-compose -f docker-compose.prod.yml restart

# Stop tất cả services
docker-compose -f docker-compose.prod.yml down

# Update ứng dụng
git pull
docker-compose -f docker-compose.prod.yml up -d --build

# View real-time logs
docker-compose -f docker-compose.prod.yml logs -f

# Backup database manual
./backup.sh

# Check system health
./monitor.sh

# Access SQL Server
docker exec -it autoapp_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "AutoApp@Production@2024"
```

## URLs sau khi deploy thành công:

- **Web Application**: https://tlsoftware.io.vn
- **API**: https://api.tlsoftware.io.vn
- **API Health Check**: https://api.tlsoftware.io.vn/health

## Liên hệ hỗ trợ

Nếu gặp vấn đề trong quá trình deploy, vui lòng:
1. Kiểm tra logs: `docker-compose logs`
2. Chạy health check: `./monitor.sh`
3. Kiểm tra system resources: `htop`, `df -h`

---

**Lưu ý**: 
- Backup thường xuyên trước khi update
- Monitor system resources
- Kiểm tra SSL certificate expiry
- Update security patches định kỳ
