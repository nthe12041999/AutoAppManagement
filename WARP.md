# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Architecture

AutoAppManagement is a comprehensive .NET 8.0 multi-layered application management system with ASP.NET Core. It follows clean architecture principles with clear separation of concerns.

### Solution Structure

The solution is organized into 5 main projects:

- **AutoAppManagement.WebApp** - MVC web application (main client UI)  
- **AutoAppManagement.API** - RESTful API layer with Swagger documentation
- **AutoAppManagement.Models** - Shared data models, DTOs, and business entities
- **AutoAppManagement.Repository** - Data access layer with Entity Framework Core
- **AutoAppManagement.Service** - Business logic layer

### Key Architecture Patterns

- **Clean Architecture**: Clear separation between layers with dependency injection
- **Repository Pattern**: Data access abstraction through repositories
- **Unit of Work**: Transaction management across repositories  
- **DTO Pattern**: Data transfer objects for API communication
- **Dependency Injection**: Built-in .NET DI container throughout
- **Multi-tenancy**: Role-based access control with feature management

### Core Features

- **Account Management**: User accounts with device tracking
- **Admin Management**: Administrative user management
- **Role-Based Access Control**: Roles, permissions, and role assignments
- **License Management**: License tracking and validation
- **Feature Management**: Simple feature toggle system (replaced deprecated tool management)
- **Real-time Notifications**: SignalR-based notification system
- **JWT Authentication**: Token-based API authentication
- **Rate Limiting**: DDOS protection with AspNetCoreRateLimit

## Common Development Commands

### Build and Run

```powershell
# Build entire solution
dotnet build AutoAppManagement.sln

# Run WebApp (main application)
dotnet run --project AutoAppManagement/AutoAppManagement.WebApp.csproj --urls https://localhost:7000

# Run API separately  
dotnet run --project AutoAppManagement.API/AutoAppManagement.API.csproj

# Build for release
dotnet build AutoAppManagement.sln -c Release
```

### Database Operations

```powershell
# Add new migration (run from API project directory)
cd AutoAppManagement.API
dotnet ef migrations add MigrationName --context AutoAppManagementContext

# Update database
dotnet ef database update --context AutoAppManagementContext

# Drop database (dev only)
dotnet ef database drop --context AutoAppManagementContext --force
```

### Docker Deployment

```powershell
# Development environment
docker-compose up -d

# Production environment
docker-compose -f docker-compose.production.yml up -d

# Build only
docker-compose -f docker-compose-build.yml build
```

### Testing and Linting

```powershell
# Run all tests (when test projects exist)
dotnet test AutoAppManagement.sln

# Format code
dotnet format AutoAppManagement.sln

# Restore packages
dotnet restore AutoAppManagement.sln
```

## Development Workflow

### Adding New Features

1. **Model Layer**: Create entities in `AutoAppManagement.Models/BaseEntity/`
2. **Repository Layer**: Add repositories in `AutoAppManagement.Repository/Repositories/`
3. **Service Layer**: Implement business logic in `AutoAppManagement.Service/Services/`
4. **API Layer**: Create controllers in `AutoAppManagement.API/Controllers/`
5. **WebApp Layer**: Add MVC controllers and views in `AutoAppManagement/Controllers/` and `Views/`

### Database Changes

All EF Core migrations are managed from the API project since it contains the `AutoAppManagementContext` and migration assembly configuration.

### Authentication Flow

- **WebApp**: Uses cookie-based authentication (`CookieAuthenticationDefaults`)
- **API**: Uses JWT Bearer tokens for API requests
- **SignalR**: Supports JWT token authentication via query string for real-time features

## Important Configuration

### Connection Strings
- Uses SQL Server with Entity Framework Core 9.0.7
- Connection string configured through `appsettings.json` or environment variables
- Production uses external SQL Server at `125.253.121.206,1433`

### Key Dependencies
- **.NET 8.0** target framework
- **Entity Framework Core 9.0.7** for data access  
- **AutoMapper 12.0.1** for object mapping
- **AspNetCoreRateLimit 5.0.0** for DDOS protection
- **Swashbuckle 9.0.3** for API documentation
- **Dapper 2.1.66** for raw SQL queries when needed

### Environment Configuration
- **Development**: Swagger enabled, detailed error pages, Razor runtime compilation
- **Production**: Swagger disabled, custom error handling, optimized builds

## Project Rules from Copilot Instructions

### Coding Standards
- Follow C# naming conventions (PascalCase for classes/methods/properties, camelCase for variables)
- Use async/await patterns for database operations  
- Implement proper error handling and logging
- Follow SOLID principles and clean architecture patterns

### API Development
- Follow RESTful conventions with proper HTTP status codes
- Implement proper authentication and authorization
- Use model validation attributes
- Document APIs with XML comments

### Security Considerations  
- Always validate user input
- Use HTTPS for all communications
- Implement proper CORS policies
- Use secure session management and rate limiting

### Communication Guidelines
- **Language**: Always respond in Vietnamese (Tiếng Việt) when explaining concepts
- **Code Comments**: Write code comments in Vietnamese for complex logic
- **Variable Names**: Use English (standard practice) but provide Vietnamese explanations
- **Documentation**: All explanations should be in Vietnamese

### File Naming Conventions
- Controllers: `{Feature}Controller.cs`
- Services: `{Feature}Service.cs` and `I{Feature}Service.cs`  
- Repositories: `{Feature}Repository.cs` and `I{Feature}Repository.cs`
- Models: `{Feature}Model.cs` or `{Feature}Dto.cs`
- ViewModels: `{Feature}ViewModel.cs`

## Development Environment Setup

The project is configured to run on `https://localhost:7000` for the main WebApp. The API runs separately and can be accessed through Swagger documentation when in development mode.

For containerized development, the Docker setup provides:
- API container on port 8081 (mapped to internal 8080)
- WebApp container on port 8080  
- Internal Docker network for service communication
- Health check endpoints at `/health` and `/ready`