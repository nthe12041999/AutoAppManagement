#!/bin/bash

echo "=== AUTO APP MANAGEMENT DEPLOY SCRIPT ==="
echo "Bắt đầu deploy production..."

# Đường dẫn tới docker-compose production  
COMPOSE_FILE="docker/docker-compose.production.yml"

# Kiểm tra file docker-compose có tồn tại không
if [ ! -f "$COMPOSE_FILE" ]; then
    echo "⚠️  File $COMPOSE_FILE không tồn tại. Sử dụng docker-compose.yml mặc định"
    COMPOSE_FILE="docker-compose.yml"
fi

# Dừng và xóa các container cũ
echo "🔄 Dừng các container hiện tại..."
docker-compose -f $COMPOSE_FILE down --remove-orphans

# Xóa images cũ để rebuild từ đầu (optional)
echo "🧹 Xóa images cũ..."
docker image rm autoappmanagement_api autoappmanagement_webapp >/dev/null 2>&1 || true
docker image rm autoappmanagement-api autoappmanagement-webapp >/dev/null 2>&1 || true

# Xóa các image dangling (unnamed)
echo "🗑️  Xóa các image dangling..."
docker image prune -f >/dev/null 2>&1

# Build và chạy containers
echo "🏗️  Build và khởi động containers..."
docker-compose -f $COMPOSE_FILE up --build -d

# Kiểm tra trạng thái
echo "📊 Kiểm tra trạng thái containers..."
docker-compose -f $COMPOSE_FILE ps

# Chờ containers khởi động
echo "⏳ Chờ containers khởi động..."
sleep 20

# Kiểm tra logs
echo ""
echo "=== 📋 API LOGS ==="
if docker ps --format "table {{.Names}}" | grep -q "autoapp-api"; then
    docker logs autoapp-api --tail 20
elif docker ps --format "table {{.Names}}" | grep -q "autoapp-api-prod"; then
    docker logs autoapp-api-prod --tail 20
else
    echo "⚠️  Không tìm thấy container API"
fi

echo ""
echo "=== 📋 WEBAPP LOGS ==="
if docker ps --format "table {{.Names}}" | grep -q "autoapp-webapp"; then
    docker logs autoapp-webapp --tail 20
elif docker ps --format "table {{.Names}}" | grep -q "autoapp-webapp-prod"; then
    docker logs autoapp-webapp-prod --tail 20
else
    echo "⚠️  Không tìm thấy container WebApp"
fi

# Kiểm tra health check
echo ""
echo "=== 🏥 HEALTH CHECK ==="

# Xác định ports đang sử dụng
API_PORT=$(docker-compose -f $COMPOSE_FILE config | grep -A 5 "ports:" | grep -o "[0-9]*:8080" | head -1 | cut -d: -f1)
WEBAPP_PORT=$(docker-compose -f $COMPOSE_FILE config | grep -A 5 "ports:" | grep -o "[0-9]*:8080" | tail -1 | cut -d: -f1)

if [ -z "$API_PORT" ]; then
    API_PORT="8080"
fi

if [ -z "$WEBAPP_PORT" ]; then
    WEBAPP_PORT="8081"
fi

echo "Kiểm tra API health: http://localhost:$API_PORT/health"
sleep 5

if command -v curl &> /dev/null; then
    if curl -f http://localhost:$API_PORT/health >/dev/null 2>&1; then
        echo "✅ API health check: OK"
    else
        echo "❌ API health check: FAILED"
        echo "   Thử: curl -v http://localhost:$API_PORT/health"
    fi
    
    echo "Kiểm tra WebApp: http://localhost:$WEBAPP_PORT/"
    if curl -f http://localhost:$WEBAPP_PORT/ >/dev/null 2>&1; then
        echo "✅ WebApp check: OK"
    else
        echo "❌ WebApp check: FAILED"
        echo "   Thử: curl -v http://localhost:$WEBAPP_PORT/"
    fi
else
    echo "⚠️  curl không có sẵn, bỏ qua health check tự động"
fi

# Lấy IP của server
SERVER_IP=$(hostname -I | awk '{print $1}')
if [ -z "$SERVER_IP" ]; then
    SERVER_IP="localhost"
fi

echo ""
echo "=== 🎉 DEPLOY HOÀN THÀNH ==="
echo "📡 API URL: http://$SERVER_IP:$API_PORT"
echo "🌐 WebApp URL: http://$SERVER_IP:$WEBAPP_PORT"
echo "🏥 Health Check: http://$SERVER_IP:$API_PORT/health"
echo ""
echo "🔧 Các lệnh hữu ích:"
echo "  - Xem logs realtime API: docker logs -f autoapp-api"
echo "  - Xem logs realtime WebApp: docker logs -f autoapp-webapp"
echo "  - Dừng services: docker-compose -f $COMPOSE_FILE down"
echo "  - Restart services: docker-compose -f $COMPOSE_FILE restart"
echo "  - Xem trạng thái: docker-compose -f $COMPOSE_FILE ps"
echo "  - Xem tài nguyên: docker stats --no-stream"
echo ""

# Hiển thị container status cuối cùng
echo "=== 📊 CONTAINER STATUS ==="
docker-compose -f $COMPOSE_FILE ps

echo ""
echo "🚀 Deploy script hoàn thành!"