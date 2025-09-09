# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["AutoAppManagement/AutoAppManagement.WebApp.csproj", "AutoAppManagement/"]
COPY ["AutoAppManagement.API/AutoAppManagement.API.csproj", "AutoAppManagement.API/"]
COPY ["AutoAppManagement.Models/AutoAppManagement.Models.csproj", "AutoAppManagement.Models/"]
COPY ["AutoAppManagement.Repository/AutoAppManagement.Repository.csproj", "AutoAppManagement.Repository/"]
COPY ["AutoAppManagement.Service/AutoAppManagement.Service.csproj", "AutoAppManagement.Service/"]

RUN dotnet restore "AutoAppManagement/AutoAppManagement.WebApp.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/AutoAppManagement"
RUN dotnet build "AutoAppManagement.WebApp.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AutoAppManagement.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install tzdata for timezone support
RUN apt-get update && apt-get install -y tzdata && rm -rf /var/lib/apt/lists/*

# Set timezone to Vietnam
ENV TZ=Asia/Ho_Chi_Minh

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser
RUN chown -R appuser:appuser /app
USER appuser

COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AutoAppManagement.WebApp.dll"]
