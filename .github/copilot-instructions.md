# GitHub Copilot Instructions for AutoAppManagement

## Project Overview

AutoAppManagement is a comprehensive .NET application management system built with ASP.NET Core. The project consists of multiple layers including Web App, API, Models, Repository, and Service layers.

## Project Structure

- **AutoAppManagement** - Main web application (MVC)
- **AutoAppManagement.API** - RESTful API layer
- **AutoAppManagement.Models** - Data models and DTOs
- **AutoAppManagement.Repository** - Data access layer
- **AutoAppManagement.Service** - Business logic layer

## Coding Standards and Conventions

### General Guidelines
- Follow C# naming conventions (PascalCase for classes, methods, properties; camelCase for variables)
- Use async/await patterns for database operations
- Implement proper error handling and logging
- Follow SOLID principles and clean architecture patterns

### Architecture Patterns
- **Repository Pattern**: Use for data access abstraction
- **Service Layer**: Implement business logic in service classes
- **Dependency Injection**: Use built-in .NET DI container
- **DTO Pattern**: Use DTOs for API communication

### Database Operations
- Use Entity Framework Core for ORM
- Implement proper migrations for database changes
- Use repository pattern for data access
- Always use parameterized queries to prevent SQL injection

### API Development
- Follow RESTful conventions
- Use proper HTTP status codes
- Implement proper authentication and authorization
- Use model validation attributes
- Document APIs with XML comments

### Authentication & Authorization
- The project appears to use custom authentication system
- Implement proper role-based access control
- Use secure password hashing
- Implement JWT tokens for API authentication

### Error Handling
- Use try-catch blocks appropriately
- Log errors with sufficient detail
- Return meaningful error responses
- Use custom exception classes when needed

### Code Organization
- Keep controllers thin - delegate business logic to services
- Use meaningful class and method names
- Group related functionality in appropriate namespaces
- Follow single responsibility principle

## Common Patterns in This Project

### Controllers
```csharp
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    private readonly IYourService _yourService;
    
    public YourController(IYourService yourService)
    {
        _yourService = yourService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // Implementation
    }
}
```

### Services
```csharp
public interface IYourService
{
    Task<Result<YourDto>> GetAsync(int id);
}

public class YourService : IYourService
{
    private readonly IYourRepository _repository;
    
    public YourService(IYourRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<YourDto>> GetAsync(int id)
    {
        // Implementation
    }
}
```

### Repositories
```csharp
public interface IYourRepository
{
    Task<YourEntity> GetByIdAsync(int id);
    Task<IEnumerable<YourEntity>> GetAllAsync();
    Task<YourEntity> AddAsync(YourEntity entity);
    Task UpdateAsync(YourEntity entity);
    Task DeleteAsync(int id);
}
```

## Security Considerations
- Always validate user input
- Use HTTPS for all communications
- Implement proper CORS policies
- Use secure session management
- Implement rate limiting for APIs
- Sanitize data before displaying

## Testing Guidelines
- Write unit tests for business logic
- Use integration tests for API endpoints
- Mock external dependencies
- Test both success and failure scenarios

## Performance Considerations
- Use async/await for I/O operations
- Implement proper caching strategies
- Use pagination for large data sets
- Optimize database queries
- Consider using background services for heavy operations

## When Working with This Codebase
1. Always check existing patterns before implementing new features
2. Maintain consistency with existing code style
3. Add proper logging for new features
4. Update documentation when adding new APIs
5. Consider backward compatibility when making changes
6. Test thoroughly before committing changes

## File Naming Conventions
- Controllers: `{Feature}Controller.cs`
- Services: `{Feature}Service.cs` and `I{Feature}Service.cs`
- Repositories: `{Feature}Repository.cs` and `I{Feature}Repository.cs`
- Models: `{Feature}Model.cs` or `{Feature}Dto.cs`
- ViewModels: `{Feature}ViewModel.cs`

## Common Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (likely)
- AutoMapper (for object mapping)
- JWT Authentication
- Swagger/OpenAPI for API documentation

## Development Environment
- Runs on HTTPS port 7000 for main application
- Uses SQL Server for database
- Postman collections available for API testing
- PowerShell scripts for database migrations

## Communication Guidelines
- **Language**: Always respond in Vietnamese (Tiếng Việt)
- **Code Comments**: Write code comments in Vietnamese when explaining complex logic
- **Variable Names**: Use English for variable/method names (standard practice) but explain in Vietnamese
- **Documentation**: All explanations and documentation should be in Vietnamese
- **Error Messages**: Provide error explanations in Vietnamese

## Quy tắc giao tiếp
- Luôn trả lời và giải thích bằng tiếng Việt
- Sử dụng thuật ngữ kỹ thuật phù hợp trong ngữ cảnh Việt Nam
- Khi giải thích code, hãy mô tả logic bằng tiếng Việt
- Đưa ra gợi ý và khuyến nghị bằng tiếng Việt
- Giúp debug và troubleshoot bằng tiếng Việt

## File Encoding Rules (QUAN TRỌNG ⚠️)
### **Khi sửa file, PHẢI tuân thủ encoding sau:**

#### **C# Files (.cs)**
- **Encoding**: UTF-8 with BOM
- **Line Ending**: CRLF (\r\n)
- **Reason**: Visual Studio và .NET compiler yêu cầu BOM cho C# files

#### **Razor Files (.cshtml, .razor)**
- **Encoding**: UTF-8 with BOM
- **Line Ending**: CRLF (\r\n)
- **Reason**: ASP.NET Core Razor engine yêu cầu BOM để hiển thị đúng ký tự tiếng Việt

#### **JavaScript/TypeScript Files (.js, .ts, .jsx, .tsx)**
- **Encoding**: UTF-8 **WITHOUT BOM**
- **Line Ending**: CRLF (\r\n)
- **Reason**: Browsers và Node.js không cần BOM, BOM có thể gây lỗi parsing

#### **JSON Files (.json)**
- **Encoding**: UTF-8 **WITHOUT BOM**
- **Line Ending**: CRLF (\r\n)
- **Reason**: JSON spec không hỗ trợ BOM

#### **Config Files (.xml, .config)**
- **Encoding**: UTF-8 with BOM
- **Line Ending**: CRLF (\r\n)

#### **Markdown Files (.md)**
- **Encoding**: UTF-8 **WITHOUT BOM**
- **Line Ending**: CRLF (\r\n)

### **Cách kiểm tra encoding trước khi sửa file:**
1. Đọc file header để xác định BOM:
   - UTF-8 with BOM: Bắt đầu với bytes `EF BB BF`
   - UTF-8 without BOM: Không có header đặc biệt
2. Khi sử dụng `replace_string_in_file`, đảm bảo không thay đổi encoding
3. Nếu không chắc chắn, hỏi user trước khi sửa file

### **Cảnh báo khi sửa file:**
- ⚠️ **KHÔNG BAO GIỜ** thay đổi encoding của file khi sửa
- ⚠️ Nếu file có ký tự tiếng Việt, kiểm tra kỹ encoding trước khi sửa
- ⚠️ Nếu không thể giữ nguyên encoding, thông báo cho user
- ⚠️ Test lại file sau khi sửa để đảm bảo ký tự tiếng Việt không bị lỗi
