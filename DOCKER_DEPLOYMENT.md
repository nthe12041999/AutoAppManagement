# AutoAppManagement Docker Deployment Guide

## Prerequisites

1. **Docker** và **Docker Compose** đã được cài đặt trên VPS
2. **Domain name** đã được cấu hình để trỏ về VPS
3. **SSL Certificate** (có thể sử dụng Let's Encrypt)
4. **Database server** (SQL Server) đã được setup

## Quick Start

### 1. Clone source code lên VPS
```bash
git clone <your-repo-url> /opt/autoappmanagement
cd /opt/autoappmanagement
```

### 2. Cấu hình environment variables
```bash
cp .env.example .env
nano .env
```

Cập nhật các thông tin trong file `.env`:
- Database connection string
- Domain names
- SSL certificate paths
- Email settings
- JWT secret key

### 3. Cấu hình SSL certificate
```bash
# Tạo thư mục SSL
mkdir -p nginx/ssl

# Copy SSL certificate vào thư mục
cp /path/to/your/cert.pem nginx/ssl/
cp /path/to/your/key.pem nginx/ssl/

# Hoặc sử dụng Let's Encrypt
certbot certonly --standalone -d yourdomain.com -d api.yourdomain.com
cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem nginx/ssl/cert.pem
cp /etc/letsencrypt/live/yourdomain.com/privkey.pem nginx/ssl/key.pem
```

### 4. Cập nhật Nginx configuration
```bash
nano nginx/nginx.conf
```
- Thay đổi `yourdomain.com` thành domain thực của bạn
- Cập nhật SSL certificate paths nếu cần

### 5. Deploy application

#### Option A: Sử dụng script (Linux)
```bash
chmod +x deploy.sh
./deploy.sh prod
```

#### Option B: Sử dụng script (Windows)
```cmd
deploy.bat prod
```

#### Option C: Manual deployment
```bash
# Với database local (development)
docker-compose up -d --build

# Với database external (production)
export DB_CONNECTION_STRING="Server=your-db-server;Database=AutoAppManagement;User Id=user;Password=pass;TrustServerCertificate=true;Encrypt=false;"
docker-compose -f docker-compose.prod.yml up -d --build
```

## Docker Files Structure

```
├── Dockerfile              # Web Application container
├── Dockerfile.api          # API container
├── docker-compose.yml      # Development with local SQL Server
├── docker-compose.prod.yml # Production with external database
├── .dockerignore           # Files to ignore during build
├── deploy.sh               # Linux deployment script
├── deploy.bat              # Windows deployment script
├── .env.example            # Environment variables template
└── nginx/
    ├── nginx.conf          # Nginx reverse proxy configuration
    └── ssl/                # SSL certificates directory
```

## Services

### 1. Web Application (`webapp`)
- **Port**: 8080
- **URL**: https://yourdomain.com
- **Container**: autoapp_webapp

### 2. API Application (`api`)
- **Port**: 8081  
- **URL**: https://api.yourdomain.com
- **Container**: autoapp_api

### 3. Nginx Reverse Proxy (`nginx`)
- **Ports**: 80, 443
- **Container**: autoapp_nginx
- **Features**: SSL termination, CORS, Rate limiting, Gzip compression

### 4. SQL Server (`sqlserver`) - Development only
- **Port**: 1433
- **Container**: autoapp_sqlserver
- **Credentials**: sa / AutoApp@123456

### 5. Redis (`redis`) - Optional
- **Port**: 6379
- **Container**: autoapp_redis
- **Password**: AutoApp@123456

## Management Commands

### Check running containers
```bash
docker-compose ps
```

### View logs
```bash
# All services
docker-compose logs

# Specific service
docker-compose logs webapp
docker-compose logs api
docker-compose logs nginx
```

### Stop services
```bash
docker-compose down
```

### Restart services
```bash
docker-compose restart
```

### Update application
```bash
# Pull latest code
git pull

# Rebuild and restart
docker-compose down
docker-compose up -d --build
```

### Database migrations
```bash
# Run migrations on API container
docker-compose exec api dotnet ef database update
```

### Backup database (if using local SQL Server)
```bash
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P AutoApp@123456 -Q "BACKUP DATABASE AutoAppManagement TO DISK = '/var/opt/mssql/backup/autoapp_$(date +%Y%m%d_%H%M%S).bak'"
```

## Production Deployment Checklist

### Before Deployment
- [ ] Database server đã được setup và accessible
- [ ] Domain names đã trỏ về VPS IP
- [ ] SSL certificates đã được chuẩn bị
- [ ] Environment variables đã được cấu hình đúng
- [ ] Firewall rules đã được setup (ports 80, 443)

### After Deployment
- [ ] Test web application: https://yourdomain.com
- [ ] Test API endpoints: https://api.yourdomain.com/health
- [ ] Check SSL certificate validity
- [ ] Verify database connections
- [ ] Test authentication and authorization
- [ ] Setup monitoring and logging
- [ ] Configure backup strategy

## Security Considerations

1. **SSL/TLS**: Sử dụng SSL certificates hợp lệ
2. **Database**: Sử dụng strong passwords và restrict network access
3. **JWT**: Sử dụng strong secret key và appropriate expiration
4. **Rate Limiting**: Nginx đã được cấu hình rate limiting
5. **CORS**: Chỉ cho phép trusted domains
6. **Container Security**: Run containers as non-root user
7. **Secrets Management**: Không commit secrets vào git

## Monitoring và Logging

### Health Checks
- Web App: https://yourdomain.com/health
- API: https://api.yourdomain.com/health

### Log Files
```bash
# Application logs
docker-compose logs -f webapp
docker-compose logs -f api

# Nginx access logs
docker-compose exec nginx tail -f /var/log/nginx/access.log

# Nginx error logs  
docker-compose exec nginx tail -f /var/log/nginx/error.log
```

## Troubleshooting

### Common Issues

1. **Database connection errors**
   ```bash
   # Check database connectivity
   docker-compose exec api dotnet ef database update --verbose
   ```

2. **SSL certificate errors**
   ```bash
   # Verify certificate files
   openssl x509 -in nginx/ssl/cert.pem -text -noout
   ```

3. **Nginx configuration errors**
   ```bash
   # Test nginx config
   docker-compose exec nginx nginx -t
   ```

4. **Container startup issues**
   ```bash
   # Check container logs
   docker-compose logs <service-name>
   
   # Check container resource usage
   docker stats
   ```

### Performance Optimization

1. **Database**: Optimize queries, use indexes, enable connection pooling
2. **Caching**: Enable Redis for session and data caching
3. **CDN**: Use CDN for static assets
4. **Compression**: Nginx Gzip is already enabled
5. **Load Balancing**: Scale horizontally with multiple container instances

## Backup and Recovery

### Database Backup
```bash
# Create backup script
cat > backup.sh << 'EOF'
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P AutoApp@123456 -Q "BACKUP DATABASE AutoAppManagement TO DISK = '/var/opt/mssql/backup/autoapp_$DATE.bak'"
EOF

chmod +x backup.sh

# Setup cron job for daily backup
echo "0 2 * * * /opt/autoappmanagement/backup.sh" | crontab -
```

### Application Backup
```bash
# Backup application files and configuration
tar -czf autoapp_backup_$(date +%Y%m%d).tar.gz \
  .env nginx/ logs/ --exclude=logs/*.log
```

## Scaling

### Horizontal Scaling
```yaml
# In docker-compose.yml, scale specific services
services:
  webapp:
    deploy:
      replicas: 3
  api:
    deploy:
      replicas: 2
```

### Load Balancing
```nginx
# In nginx.conf, add multiple upstream servers
upstream webapp_backend {
    server webapp_1:8080;
    server webapp_2:8080;
    server webapp_3:8080;
}
```
