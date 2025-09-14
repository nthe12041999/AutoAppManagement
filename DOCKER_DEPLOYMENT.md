# AutoAppManagement - Docker Deployment Guide

## Cấu trúc Docker

```
├── docker/
│   ├── api.Dockerfile              # Dockerfile cho API
│   ├── webapp.Dockerfile           # Dockerfile cho WebApp
│   ├── docker-compose.production.yml # Production compose
│   └── .dockerignore               # Ignore files
├── docker-compose.yml              # Development compose
├── deploy-production.bat           # Deploy script cho Windows
├── deploy-dev.bat                  # Deploy script development
└── stop-production.bat             # Script dừng services
```

## Deploy Production

### Tự động (Recommended)
```bash
# Windows
deploy-production.bat

# Linux/Mac (cần tạo script .sh tương tự)
chmod +x deploy-production.sh
./deploy-production.sh
```

### Thủ công
```bash
# Build và deploy
docker-compose -f docker/docker-compose.production.yml up --build -d

# Kiểm tra trạng thái
docker-compose -f docker/docker-compose.production.yml ps

# Xem logs
docker logs autoapp-api-prod -f
docker logs autoapp-webapp-prod -f
```

## Deploy Development

```bash
# Windows
deploy-dev.bat

# Manual
docker-compose up --build -d
```

## URLs sau khi deploy

- **API**: http://tlsoftware.io.vn (Port 80)
- **WebApp**: http://tlsoftware.io.vn:8080
- **API Health Check**: http://localhost:80/health
- **API Ready Check**: http://localhost:80/ready

## Dừng Services

```bash
# Windows
stop-production.bat

# Manual
docker-compose -f docker/docker-compose.production.yml down
```

## Troubleshooting

### Kiểm tra logs
```bash
# API logs
docker logs autoapp-api-prod --tail 50

# WebApp logs  
docker logs autoapp-webapp-prod --tail 50
```

### Kiểm tra health
```bash
# Health check
curl http://localhost:80/health

# WebApp check
curl http://localhost:8080/
```

### Restart services
```bash
# Restart tất cả
docker-compose -f docker/docker-compose.production.yml restart

# Restart từng service
docker-compose -f docker/docker-compose.production.yml restart api
docker-compose -f docker/docker-compose.production.yml restart webapp
```

### Rebuild từ đầu
```bash
# Xóa containers và images cũ
docker-compose -f docker/docker-compose.production.yml down --rmi all --volumes

# Build lại
docker-compose -f docker/docker-compose.production.yml up --build -d
```

## Cấu hình Production

### Database Connection
- Host: 125.253.121.206:1433
- Database: AutoAppManagement
- Connection được cấu hình trong docker-compose.production.yml

### Environment Variables
Các biến environment quan trọng:
- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection`: Connection string database
- `Jwt__SecretKey`: JWT secret key
- `AppSettings__BaseUrl`: Base URL cho application

### Ports
- API: Container 80 → Host 80
- WebApp: Container 80 → Host 8080

### Resources
- API: Max 1GB RAM, 1 CPU core
- WebApp: Max 512MB RAM, 0.5 CPU core

## Monitoring

### Health Checks
- API có health check endpoint tại `/health`
- Containers tự động restart nếu health check fail
- Logs được giới hạn 10MB × 3 files

### Logs
```bash
# Real-time logs
docker logs -f autoapp-api-prod
docker logs -f autoapp-webapp-prod

# Export logs
docker logs autoapp-api-prod > api-logs.txt
docker logs autoapp-webapp-prod > webapp-logs.txt
```

## Security Notes

- Production sử dụng HTTPS trong thực tế (cần reverse proxy như Nginx)
- Database connection sử dụng encrypted connection
- JWT tokens có thời hạn 24h
- CORS được cấu hình cho domain tlsoftware.io.vn

## Backup & Restore

### Database Backup
```sql
-- Connect to SQL Server và chạy
BACKUP DATABASE AutoAppManagement 
TO DISK = 'C:\Backup\AutoAppManagement.bak'
```

### Container Backup
```bash
# Tạo image từ container đang chạy
docker commit autoapp-api-prod autoapp-api-backup:$(date +%Y%m%d)
docker commit autoapp-webapp-prod autoapp-webapp-backup:$(date +%Y%m%d)
```