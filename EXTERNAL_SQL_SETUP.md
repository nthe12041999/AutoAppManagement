# Hướng dẫn Deploy với SQL Server có sẵn

## Tổng quan
Hướng dẫn này dành cho trường hợp VPS đã có SQL Server cài sẵn và chạy trên port 1433.

## Các bước thực hiện

### Bước 1: Setup Database
```bash
chmod +x setup-external-database.sh
./setup-external-database.sh
```

Script này sẽ:
- Kiểm tra kết nối SQL Server
- Tạo database `AutoAppManagement`
- Tạo user `autoapp_user` với password `AutoApp@User@2024`
- Cấp quyền cần thiết
- Tạo connection string

### Bước 2: Setup SSL (nếu chưa có)
```bash
chmod +x setup-ssl.sh
./setup-ssl.sh
```

### Bước 3: Deploy ứng dụng
```bash
chmod +x deploy-external-sql.sh
./deploy-external-sql.sh
```

## Cấu hình

### Docker Compose
- File: `docker-compose.external-sql.yml`
- Không có SQL Server container
- Sử dụng `host.docker.internal` để kết nối SQL Server

### Connection String
```
Server=host.docker.internal,1433;Database=AutoAppManagement;User Id=autoapp_user;Password=AutoApp@User@2024;TrustServerCertificate=true;Encrypt=false;
```

### Network Configuration
- Container sử dụng `extra_hosts` để truy cập host
- SQL Server accessible qua `host.docker.internal:1433`

## Troubleshooting

### 1. Lỗi kết nối SQL Server
```bash
# Test từ host
sqlcmd -S localhost -U autoapp_user -P "AutoApp@User@2024" -d AutoAppManagement

# Test từ container
docker exec -it autoapp_api bash
# Trong container:
ping host.docker.internal
```

### 2. Kiểm tra SQL Server configuration
```sql
-- Enable TCP/IP protocol
-- Enable Mixed Mode Authentication
-- Check port 1433 is listening

-- Check from SQL Server
SELECT name FROM sys.databases WHERE name = 'AutoAppManagement';
SELECT name FROM sys.sql_logins WHERE name = 'autoapp_user';
```

### 3. Firewall issues
```bash
# Check if port 1433 accessible from containers
sudo netstat -tulpn | grep :1433

# Allow Docker to access SQL Server
sudo ufw allow from 172.17.0.0/16 to any port 1433
```

## Monitoring

### Health Check
```bash
./monitor-external-sql.sh
```

### Manual checks
```bash
# Container status
docker-compose -f docker-compose.external-sql.yml ps

# Application logs
docker-compose -f docker-compose.external-sql.yml logs api

# SQL Server connection test
sqlcmd -S localhost -U autoapp_user -P "AutoApp@User@2024" -Q "SELECT @@VERSION"
```

## Backup

### Automated backup
```bash
./backup-external-sql.sh
```

### Manual backup
```sql
BACKUP DATABASE [AutoAppManagement] 
TO DISK = '/opt/autoappmanagement/backups/manual_backup.bak'
```

## Security Notes

1. **Change default passwords** sau khi setup
2. **Restrict network access** chỉ containers cần thiết
3. **Use Windows Authentication** nếu có thể
4. **Enable SQL Server encryption**
5. **Regular security updates**

## Useful Commands

```bash
# Restart only app containers (not SQL Server)
docker-compose -f docker-compose.external-sql.yml restart webapp api

# View real-time logs
docker-compose -f docker-compose.external-sql.yml logs -f

# Connect to SQL Server from host
sqlcmd -S localhost -U autoapp_user -P "AutoApp@User@2024" -d AutoAppManagement

# Update application
git pull
docker-compose -f docker-compose.external-sql.yml up -d --build

# Check connection from API container
docker-compose -f docker-compose.external-sql.yml exec api dotnet ef database update
```

## Configuration Files

- `docker-compose.external-sql.yml` - Main compose file
- `.env.external-sql` - Database connection string
- `setup-external-database.sh` - Database setup script
- `deploy-external-sql.sh` - Deployment script
- `monitor-external-sql.sh` - Monitoring script
- `backup-external-sql.sh` - Backup script

## URLs

- **Web App**: https://tlsoftware.io.vn
- **API**: https://api.tlsoftware.io.vn
- **API Health**: https://api.tlsoftware.io.vn/health
- **SQL Server**: localhost:1433 (from host)
