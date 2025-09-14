@echo off
echo === AUTO APP MANAGEMENT - LOCAL DEVELOPMENT DEPLOY ===
echo Khởi động containers cho development...

:: Dừng và xóa các container cũ
echo Dừng các container hiện tại...
docker-compose down --remove-orphans

:: Build và chạy containers
echo Build và khởi động containers...
docker-compose up --build -d

:: Kiểm tra trạng thái
echo Kiểm tra trạng thái containers...
docker-compose ps

:: Chờ một chút để containers khởi động
echo Chờ containers khởi động...
timeout /t 10 /nobreak >nul

:: Kiểm tra logs
echo === API LOGS ===
docker logs autoapp-api --tail 20

echo.
echo === WEBAPP LOGS ===
docker logs autoapp-web --tail 20

echo.
echo === DEVELOPMENT ENVIRONMENT READY ===
echo API URL: http://localhost:8080
echo WebApp URL: http://localhost:8081
echo.
echo Để xem logs realtime: docker logs -f autoapp-api
echo Để dừng services: docker-compose down
pause