@echo off
echo === AUTO APP MANAGEMENT DEPLOY SCRIPT ===
echo Bat dau deploy production...

:: Duong dan toi docker-compose production
set COMPOSE_FILE=docker\docker-compose.production.yml

:: Dung va xoa cac container cu
echo Dung cac container hien tai...
docker-compose -f %COMPOSE_FILE% down --remove-orphans

:: Xoa images cu de rebuild tu dau (optional - co the comment neu muon build nhanh)
echo Xoa images cu...
docker image rm autoappmanagement_api autoappmanagement_webapp >nul 2>&1

:: Xoa cac image dangling (unnamed)
echo Xoa cac image dangling...
docker image prune -f >nul 2>&1

:: Build va chay containers
echo Build va khoi dong containers...
docker-compose -f %COMPOSE_FILE% up --build -d

:: Kiem tra trang thai
echo Kiem tra trang thai containers...
docker-compose -f %COMPOSE_FILE% ps

:: Cho mot chut de containers khoi dong
echo Cho containers khoi dong...
timeout /t 15 /nobreak >nul

:: Kiem tra logs
echo === API LOGS ===
docker logs autoapp-api-prod --tail 20

echo.
echo === WEBAPP LOGS ===
docker logs autoapp-webapp-prod --tail 20

:: Kiem tra health check
echo.
echo === HEALTH CHECK ===
echo Kiem tra API health: http://localhost:80/health
timeout /t 5 /nobreak >nul
curl -f http://localhost:80/health
if %ERRORLEVEL% EQU 0 (
    echo API health check: OK
) else (
    echo API health check: FAILED
)

echo Kiem tra WebApp: http://localhost:8080/
timeout /t 5 /nobreak >nul
curl -f http://localhost:8080/
if %ERRORLEVEL% EQU 0 (
    echo WebApp check: OK
) else (
    echo WebApp check: FAILED
)

echo.
echo === DEPLOY HOAN THANH ===
echo API URL: http://tlsoftware.io.vn (Port 80)
echo WebApp URL: http://tlsoftware.io.vn:8080
echo.
echo Cac lenh huu ich:
echo - Xem logs realtime API: docker logs -f autoapp-api-prod
echo - Xem logs realtime WebApp: docker logs -f autoapp-webapp-prod
echo - Dung services: docker-compose -f %COMPOSE_FILE% down
echo - Restart services: docker-compose -f %COMPOSE_FILE% restart
echo - Xem trang thai: docker-compose -f %COMPOSE_FILE% ps
echo.
pause