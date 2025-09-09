#!/bin/bash

# Script kiểm tra port usage và cleanup

echo "🔍 Checking port 1433 usage..."

# Check what's using port 1433
echo "=== Processes using port 1433 ==="
sudo netstat -tulpn | grep :1433 || echo "No process found using port 1433"
sudo lsof -i :1433 || echo "No process found using port 1433"

echo ""
echo "=== Docker containers status ==="
docker ps -a

echo ""
echo "=== Checking for SQL Server containers ==="
docker ps -a | grep -i sql || echo "No SQL Server containers found"

echo ""
echo "=== Available ports around 1433 ==="
for port in 1432 1433 1434 1435; do
    if netstat -tuln | grep -q ":$port "; then
        echo "Port $port: USED"
    else
        echo "Port $port: FREE"
    fi
done

echo ""
echo "=== Cleanup commands ==="
echo "To stop all containers: docker stop \$(docker ps -aq)"
echo "To remove all containers: docker rm \$(docker ps -aq)"
echo "To stop specific SQL container: docker stop autoapp_sqlserver"
echo "To remove specific SQL container: docker rm autoapp_sqlserver"

echo ""
echo "=== Current Docker Compose status ==="
if [[ -f "docker-compose.prod.yml" ]]; then
    docker-compose -f docker-compose.prod.yml ps || echo "Docker compose not running"
else
    echo "docker-compose.prod.yml not found"
fi
