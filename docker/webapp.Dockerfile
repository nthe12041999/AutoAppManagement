# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln ./

# Copy all project files for restore (giữ nguyên cấu trúc thư mục)
COPY AutoAppManagement/*.csproj AutoAppManagement/
COPY AutoAppManagement.API/*.csproj AutoAppManagement.API/
COPY AutoAppManagement.Service/*.csproj AutoAppManagement.Service/
COPY AutoAppManagement.Repository/*.csproj AutoAppManagement.Repository/
COPY AutoAppManagement.Models/*.csproj AutoAppManagement.Models/

# Restore dependencies cho WebApp
RUN dotnet restore AutoAppManagement/AutoAppManagement.WebApp.csproj

# Copy toàn bộ source code
COPY . .

# Build và publish WebApp project
RUN dotnet publish AutoAppManagement/AutoAppManagement.WebApp.csproj -c Release -o /app/out /p:UseAppHost=false

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health check
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Set port cho container - mặc định 8080, có thể override bằng environment variable
ENV ASPNETCORE_URLS=http://+:8080

# Copy published files từ build stage
COPY --from=build /app/out ./

# Health check cho webapp - check port 8080 hoặc 80
HEALTHCHECK --interval=30s --timeout=10s --retries=3 --start-period=60s \
    CMD curl -f http://localhost:8080/ || curl -f http://localhost:80/ || exit 1

EXPOSE 8080
EXPOSE 80

# Run application với đúng tên assembly
ENTRYPOINT ["dotnet", "AutoAppManagement.WebApp.dll"]
