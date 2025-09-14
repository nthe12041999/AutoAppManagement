@echo off
echo === AUTO APP MANAGEMENT - STOP SERVICES ===
echo Dung cac services...

set COMPOSE_FILE=docker\docker-compose.production.yml

:: Dung tat ca services
echo Dung containers...
docker-compose -f %COMPOSE_FILE% down

:: Hien thi trang thai
echo Trang thai containers:
docker-compose -f %COMPOSE_FILE% ps

echo.
echo === SERVICES DA DUNG ===
echo Tat ca containers da duoc dung.
echo.
echo Cac lenh khac:
echo - Khoi dong lai: deploy-production.bat
echo - Xem logs: docker logs autoapp-api-prod
echo - Xoa tat ca: docker-compose -f %COMPOSE_FILE% down -v --rmi all
echo.
pause