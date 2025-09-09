@echo off
setlocal enabledelayedexpansion

REM Deploy script for AutoAppManagement on Windows VPS
REM Usage: deploy.bat [environment]
REM Environment: dev, staging, prod (default: prod)

set ENVIRONMENT=%1
if "%ENVIRONMENT%"=="" set ENVIRONMENT=prod

set PROJECT_NAME=autoappmanagement
set VERSION=%date:~-4%%date:~-10,2%%date:~-7,2%-%time:~0,2%%time:~3,2%%time:~6,2%
set VERSION=%VERSION: =0%

echo 🚀 Starting deployment for environment: %ENVIRONMENT%

REM Check if Docker is installed
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not installed!
    exit /b 1
)

REM Check if Docker Compose is installed
docker-compose --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker Compose is not installed!
    exit /b 1
)

REM Create directories if they don't exist
echo [INFO] Creating necessary directories...
if not exist "nginx\ssl" mkdir "nginx\ssl"
if not exist "Database\Scripts" mkdir "Database\Scripts"
if not exist "logs" mkdir "logs"

REM Set environment variables based on environment
if "%ENVIRONMENT%"=="dev" (
    set COMPOSE_FILE=docker-compose.yml
    set DB_CONNECTION_STRING=Server=sqlserver,1433;Database=AutoAppManagement_Dev;User Id=sa;Password=AutoApp@123456;TrustServerCertificate=true;Encrypt=false;
) else if "%ENVIRONMENT%"=="staging" (
    set COMPOSE_FILE=docker-compose.prod.yml
    set DB_CONNECTION_STRING=Server=your-staging-db-server;Database=AutoAppManagement_Staging;User Id=your-user;Password=your-password;TrustServerCertificate=true;Encrypt=false;
) else if "%ENVIRONMENT%"=="prod" (
    set COMPOSE_FILE=docker-compose.prod.yml
    set DB_CONNECTION_STRING=Server=your-prod-db-server;Database=AutoAppManagement;User Id=your-user;Password=your-password;TrustServerCertificate=true;Encrypt=false;
) else (
    echo [ERROR] Invalid environment: %ENVIRONMENT%
    exit /b 1
)

echo [INFO] Using compose file: %COMPOSE_FILE%
echo [INFO] Database connection configured for %ENVIRONMENT%

REM Stop existing containers
echo [INFO] Stopping existing containers...
docker-compose -f %COMPOSE_FILE% down --remove-orphans

REM Remove old images (optional)
echo [INFO] Cleaning up old images...
docker image prune -f

REM Build and start containers
echo [INFO] Building and starting containers...
docker-compose -f %COMPOSE_FILE% up -d --build

REM Wait for services to be ready
echo [INFO] Waiting for services to be ready...
timeout /t 30 /nobreak > nul

REM Check if services are running
echo [INFO] Checking service health...
docker-compose -f %COMPOSE_FILE% ps

REM Run database migrations (if using local SQL Server)
if "%ENVIRONMENT%"=="dev" (
    echo [INFO] Running database migrations...
    timeout /t 10 /nobreak > nul
    docker-compose -f %COMPOSE_FILE% exec api dotnet ef database update
)

echo [INFO] Deployment completed successfully! 🎉
echo [INFO] Web App: http://localhost:8080
echo [INFO] API: http://localhost:8081

REM Show running containers
echo [INFO] Running containers:
docker-compose -f %COMPOSE_FILE% ps

pause
