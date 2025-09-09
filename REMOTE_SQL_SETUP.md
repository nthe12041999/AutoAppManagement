# Hướng dẫn Deploy với Remote SQL Server

## Tổng quan
Hướng dẫn này dành cho trường hợp SQL Server nằm trên server riêng biệt với IP: `125.253.121.206:1433`

## Kiến trúc hệ thống
```
VPS AutoApp (tlsoftware.io.vn)    Remote SQL Server
├── Nginx Host (80/443)     ────────→ 125.253.121.206:1433
├── WebApp Container (8080)         └── Database: AutoAppManagement
└── API Container (8081)            └── User: autoapp_user
```

## Yêu cầu hệ thống

### VPS AutoApp
- Ubuntu 20.04+ hoặc CentOS 8+
- RAM: 2GB+ (khuyến nghị 4GB+)
- Disk: 20GB+
- Network: Kết nối internet tốt
- Ports: 80, 443, 8080, 8081

### Remote SQL Server (125.253.121.206)
- SQL Server 2017+ 
- Mixed Mode Authentication enabled
- TCP/IP protocol enabled
- Port 1433 accessible từ VPS
- Firewall whitelist VPS IP

## Các bước thực hiện

### Bước 1: Test connectivity
```bash
chmod +x test-remote-sql-connectivity.sh
./test-remote-sql-connectivity.sh
```

Script này sẽ kiểm tra:
- ✅ Network connectivity tới 125.253.121.206:1433
- ✅ Port 1433 accessibility
- ✅ DNS resolution
- ✅ Network path tracing

### Bước 2: Setup Remote Database
```bash
chmod +x setup-remote-database.sh
./setup-remote-database.sh
```

Script sẽ:
- Cài đặt SQL Server tools (sqlcmd)
- Kết nối tới remote SQL Server
- Tạo database `AutoAppManagement`
- Tạo user `autoapp_user`
- Cấp quyền cần thiết
- Tạo connection string

### Bước 3: Setup Host Nginx (nếu cần)
```bash
chmod +x setup-host-nginx.sh
./setup-host-nginx.sh
```

### Bước 4: Deploy Application
```bash
chmod +x deploy-remote-sql.sh
./deploy-remote-sql.sh
```

## Cấu hình

### Connection String
```
Server=125.253.121.206,1433;Database=AutoAppManagement;User Id=autoapp_user;Password=AutoApp@User@2024;TrustServerCertificate=true;Encrypt=false;ConnectTimeout=30;CommandTimeout=30;
```

### Docker Compose
- File: `docker-compose.remote-sql.yml`
- Không có SQL Server container
- Không cần `extra_hosts` vì kết nối external IP

### Network Flow
```
Internet → VPS Nginx (443) → Container (8080/8081) → Remote SQL (125.253.121.206:1433)
```

## Security Configuration

### SQL Server Security (125.253.121.206)
1. **Firewall Rules**:
   ```sql
   -- Chỉ cho phép VPS IP kết nối
   -- Kiểm tra VPS IP: curl ifconfig.me
   ```

2. **SQL Server Configuration**:
   ```sql
   -- Enable TCP/IP
   -- Enable Mixed Mode Authentication
   -- Set strong SA password
   -- Create dedicated application user
   ```

3. **Network Security**:
   - Chỉ mở port 1433 cho VPS IP
   - Sử dụng VPN nếu có thể
   - Monitor connection logs

### VPS Security
1. **Firewall**:
   ```bash
   sudo ufw allow 22    # SSH
   sudo ufw allow 80    # HTTP
   sudo ufw allow 443   # HTTPS
   sudo ufw enable
   ```

2. **Outbound connections**:
   - Đảm bảo VPS có thể kết nối ra port 1433
   - Test với: `telnet 125.253.121.206 1433`

## Monitoring & Maintenance

### Health Checks
```bash
# Kiểm tra tổng quan
./monitor-remote-sql.sh

# Test SQL connectivity
./test-remote-sql-connectivity.sh

# Test connection từ application
docker-compose -f docker-compose.remote-sql.yml exec api dotnet ef database update --dry-run
```

### Backup Strategy
```bash
# Backup remote database
./backup-remote-sql.sh

# Manual backup SQL command
sqlcmd -S 125.253.121.206,1433 -U autoapp_user -P "password" -Q "BACKUP DATABASE [AutoAppManagement] TO DISK = '/tmp/backup.bak'"
```

### Log Monitoring
```bash
# Application logs
docker-compose -f docker-compose.remote-sql.yml logs -f

# Nginx logs
sudo tail -f /var/log/nginx/access.log

# System logs
journalctl -f
```

## Troubleshooting

### Network Issues
```bash
# Test basic connectivity
ping 125.253.121.206

# Test port
telnet 125.253.121.206 1433
# hoặc
timeout 5 bash -c '</dev/tcp/125.253.121.206/1433'

# Check routing
traceroute 125.253.121.206
```

### SQL Server Issues
```bash
# Test authentication
sqlcmd -S 125.253.121.206,1433 -U autoapp_user -P "password" -Q "SELECT @@VERSION"

# Check database
sqlcmd -S 125.253.121.206,1433 -U autoapp_user -P "password" -d AutoAppManagement -Q "SELECT DB_NAME()"
```

### Application Issues
```bash
# Check container connectivity
docker-compose -f docker-compose.remote-sql.yml exec api ping 125.253.121.206

# Test from inside container
docker-compose -f docker-compose.remote-sql.yml exec api bash
# Inside container:
# curl -v telnet://125.253.121.206:1433
```

## Performance Optimization

### Connection Pooling
- Sử dụng connection pooling trong .NET
- Giới hạn số connection đồng thời
- Monitor connection count

### Network Optimization
- Đảm bảo latency thấp giữa VPS và SQL Server
- Sử dụng persistent connections
- Optimize query performance

### Caching Strategy
- Implement Redis cache nếu cần
- Cache frequently accessed data
- Reduce database round trips

## Emergency Procedures

### SQL Server Down
1. Check SQL Server status
2. Check network connectivity
3. Failover to backup if available
4. Notify users about maintenance

### Network Issues
1. Test alternative routes
2. Check with hosting provider
3. Monitor error rates
4. Implement circuit breaker pattern

## Scripts Available

- `test-remote-sql-connectivity.sh` - Test connectivity
- `setup-remote-database.sh` - Setup remote database
- `deploy-remote-sql.sh` - Deploy with remote SQL
- `monitor-remote-sql.sh` - Health monitoring
- `backup-remote-sql.sh` - Database backup

## URLs sau khi deploy

- **Web App**: https://tlsoftware.io.vn
- **API**: https://api.tlsoftware.io.vn
- **Health Check**: https://api.tlsoftware.io.vn/health
- **SQL Server**: 125.253.121.206:1433 (internal only)

## Support & Maintenance

### Regular Tasks
- Monitor connection health
- Check SQL Server performance
- Update security patches
- Backup verification
- Log rotation

### Weekly Tasks
- Review connection logs
- Check disk space on both servers
- Performance metrics review
- Security audit

### Monthly Tasks
- Password rotation (if required)
- Backup retention cleanup
- Performance optimization review
- Infrastructure capacity planning
