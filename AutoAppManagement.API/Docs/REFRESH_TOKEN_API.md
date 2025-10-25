# Refresh Token Management API Documentation

## Tổng quan
Tài liệu này mô tả hệ thống quản lý Refresh Token trong AutoApp Management, bao gồm:
- Cơ chế hoạt động của Refresh Token
- API endpoints để quản lý token
- Hướng dẫn tích hợp với client
- Best practices về bảo mật

## Cơ chế hoạt động

### 1. Token Lifecycle
```
1. User đăng nhập → Nhận Access Token (24h) + Refresh Token (7 ngày)
2. Access Token hết hạn → Sử dụng Refresh Token để lấy token mới
3. Refresh Token hết hạn → User phải đăng nhập lại
4. Logout → Thu hồi tất cả token
```

### 2. Token Security
- **Access Token**: JWT, thời gian sống ngắn (24h), chứa thông tin user
- **Refresh Token**: Random string, thời gian sống dài (7 ngày), lưu trong database
- **Rotation**: Mỗi lần refresh sẽ tạo token mới và vô hiệu hóa token cũ
- **Revocation**: Có thể thu hồi token bất kỳ lúc nào

## API Endpoints

### 1. Đăng nhập (Cập nhật)

#### Endpoint
```
POST /api/Account/Login
```

#### Request Body
```json
{
  "emailOrPhone": "user@example.com",
  "password": "password123"
}
```

#### Response Success
```json
{
  "isSuccess": true,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck...",
    "accessTokenExpired": "2024-10-25T10:30:00Z",
    "refreshTokenExpired": "2024-10-31T10:30:00Z",
    "loginTime": "2024-10-24T10:30:00Z",
    "licenseInfo": {
      "licenseId": 1,
      "licenseName": "Premium License",
      "licenseType": "Premium",
      "status": 1,
      "daysRemaining": 365
    },
    "availableResources": [],
    "allowedFeatures": []
  }
}
```

### 2. Refresh Token

#### Endpoint
```
POST /api/Account/RefreshToken
```

#### Request Body
```json
{
  "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck..."
}
```

#### Response Success
```json
{
  "isSuccess": true,
  "message": "Refresh token thành công",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "accessTokenExpired": "2024-10-25T11:30:00Z",
    "refreshToken": "CfDJ8N3Xx6CkOruOvBjBFdOo7dl...",
    "refreshTokenExpired": "2024-11-01T11:30:00Z"
  }
}
```

#### Response Error
```json
{
  "isSuccess": false,
  "message": "Refresh token không hợp lệ"
}
```

### 3. Thu hồi Token

#### Endpoint
```
POST /api/Account/RevokeToken
```

#### Headers
```
Authorization: Bearer <ACCESS_TOKEN>
```

#### Request Body
```json
{
  "token": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck..."
}
```

#### Response Success
```json
{
  "isSuccess": true,
  "message": "Thu hồi token thành công"
}
```

### 4. Thu hồi tất cả Token

#### Endpoint
```
POST /api/Account/RevokeAllTokens
```

#### Headers
```
Authorization: Bearer <ACCESS_TOKEN>
```

#### Response Success
```json
{
  "isSuccess": true,
  "message": "Thu hồi tất cả token thành công"
}
```

## Tích hợp Client

### 1. JavaScript/TypeScript Example

```typescript
class TokenManager {
    private accessToken: string | null = null;
    private refreshToken: string | null = null;
    private accessTokenExpiry: Date | null = null;

    constructor() {
        this.loadTokensFromStorage();
    }

    // Lưu token vào localStorage
    saveTokens(loginResponse: any) {
        this.accessToken = loginResponse.data.token;
        this.refreshToken = loginResponse.data.refreshToken;
        this.accessTokenExpiry = new Date(loginResponse.data.accessTokenExpired);
        
        localStorage.setItem('accessToken', this.accessToken);
        localStorage.setItem('refreshToken', this.refreshToken);
        localStorage.setItem('accessTokenExpiry', this.accessTokenExpiry.toISOString());
    }

    // Load token từ localStorage
    loadTokensFromStorage() {
        this.accessToken = localStorage.getItem('accessToken');
        this.refreshToken = localStorage.getItem('refreshToken');
        const expiry = localStorage.getItem('accessTokenExpiry');
        this.accessTokenExpiry = expiry ? new Date(expiry) : null;
    }

    // Kiểm tra token có hết hạn không
    isAccessTokenExpired(): boolean {
        if (!this.accessTokenExpiry) return true;
        return new Date() >= this.accessTokenExpiry;
    }

    // Refresh token tự động
    async refreshAccessToken(): Promise<boolean> {
        if (!this.refreshToken) return false;

        try {
            const response = await fetch('/api/Account/RefreshToken', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    refreshToken: this.refreshToken
                })
            });

            const result = await response.json();
            
            if (result.isSuccess) {
                this.accessToken = result.data.accessToken;
                this.refreshToken = result.data.refreshToken;
                this.accessTokenExpiry = new Date(result.data.accessTokenExpired);
                
                localStorage.setItem('accessToken', this.accessToken);
                localStorage.setItem('refreshToken', this.refreshToken);
                localStorage.setItem('accessTokenExpiry', this.accessTokenExpiry.toISOString());
                
                return true;
            }
        } catch (error) {
            console.error('Refresh token failed:', error);
        }

        this.clearTokens();
        return false;
    }

    // Lấy access token hợp lệ
    async getValidAccessToken(): Promise<string | null> {
        if (!this.isAccessTokenExpired()) {
            return this.accessToken;
        }

        const refreshed = await this.refreshAccessToken();
        return refreshed ? this.accessToken : null;
    }

    // Xóa tất cả token
    clearTokens() {
        this.accessToken = null;
        this.refreshToken = null;
        this.accessTokenExpiry = null;
        
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('accessTokenExpiry');
    }

    // Logout
    async logout() {
        if (this.refreshToken) {
            try {
                await fetch('/api/Account/RevokeAllTokens', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${this.accessToken}`,
                        'Content-Type': 'application/json'
                    }
                });
            } catch (error) {
                console.error('Revoke tokens failed:', error);
            }
        }
        
        this.clearTokens();
    }
}

// HTTP Interceptor để tự động refresh token
class ApiClient {
    private tokenManager = new TokenManager();

    async request(url: string, options: RequestInit = {}): Promise<Response> {
        const token = await this.tokenManager.getValidAccessToken();
        
        if (token) {
            options.headers = {
                ...options.headers,
                'Authorization': `Bearer ${token}`
            };
        }

        const response = await fetch(url, options);

        // Nếu 401 và có refresh token, thử refresh
        if (response.status === 401 && this.tokenManager.refreshToken) {
            const refreshed = await this.tokenManager.refreshAccessToken();
            if (refreshed) {
                // Retry request với token mới
                options.headers = {
                    ...options.headers,
                    'Authorization': `Bearer ${this.tokenManager.accessToken}`
                };
                return fetch(url, options);
            } else {
                // Redirect to login
                window.location.href = '/login';
            }
        }

        return response;
    }
}
```

### 2. C# Client Example

```csharp
public class TokenManager
{
    private string? _accessToken;
    private string? _refreshToken;
    private DateTime? _accessTokenExpiry;
    private readonly HttpClient _httpClient;

    public TokenManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_refreshToken)) return false;

        var request = new { refreshToken = _refreshToken };
        var response = await _httpClient.PostAsJsonAsync("/api/Account/RefreshToken", request);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
            if (result?.IsSuccess == true)
            {
                _accessToken = result.Data.AccessToken;
                _refreshToken = result.Data.RefreshToken;
                _accessTokenExpiry = result.Data.AccessTokenExpired;
                return true;
            }
        }

        return false;
    }

    public async Task<string?> GetValidAccessTokenAsync()
    {
        if (_accessTokenExpiry > DateTime.UtcNow)
        {
            return _accessToken;
        }

        var refreshed = await RefreshTokenAsync();
        return refreshed ? _accessToken : null;
    }
}
```

## Database Schema

### RefreshTokens Table
```sql
CREATE TABLE [RefreshTokens] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Token] nvarchar(500) NOT NULL,
    [AccountId] bigint NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL DEFAULT 0,
    [IsRevoked] bit NOT NULL DEFAULT 0,
    [ReplacedByToken] nvarchar(500) NULL,
    [CreatedByIp] nvarchar(45) NULL,
    [RevokedByIp] nvarchar(45) NULL,
    [RevokedDate] datetime2 NULL,
    [DeviceInfo] nvarchar(255) NULL,
    [UserAgent] nvarchar(255) NULL,
    [CreatedDate] datetime2 NULL DEFAULT (getdate()),
    [CreatedBy] bigint NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] bigint NULL,
    [Status] int NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_RefreshTokens_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([ID]) ON DELETE CASCADE
);
```

## Cấu hình

### appsettings.json
```json
{
  "Jwt": {
    "SecretKey": "Your_Very_Long_Secret_Key_Here",
    "Issuer": "AutoAppManagement",
    "Audience": "AutoAppManagement.Client",
    "ExpiryMinutes": "1440",
    "RefreshTokenExpiryDays": "7"
  }
}
```

## Best Practices

### 1. Bảo mật
- **Lưu trữ**: Refresh token nên lưu trong HttpOnly cookie hoặc secure storage
- **HTTPS**: Luôn sử dụng HTTPS trong production
- **Rotation**: Implement token rotation để giảm rủi ro
- **Revocation**: Cung cấp cơ chế thu hồi token khi cần thiết

### 2. Performance
- **Caching**: Cache access token trong memory
- **Batch requests**: Tránh gọi refresh token đồng thời nhiều lần
- **Cleanup**: Định kỳ dọn dẹp expired token trong database

### 3. User Experience
- **Transparent refresh**: Tự động refresh token mà không làm gián đoạn user
- **Graceful degradation**: Xử lý lỗi một cách mượt mà
- **Clear error messages**: Thông báo lỗi rõ ràng cho user

## Troubleshooting

### 1. Refresh token không hoạt động
- Kiểm tra token có hết hạn không
- Kiểm tra token có bị revoke không
- Kiểm tra account có bị khóa không

### 2. Performance issues
- Kiểm tra index trên bảng RefreshTokens
- Implement cleanup job cho expired tokens
- Monitor database query performance

### 3. Security concerns
- Audit log cho token operations
- Monitor suspicious token usage
- Implement rate limiting cho refresh endpoint

## Test Cases

### Test Case 1: Login và nhận token
```bash
curl -X POST "https://localhost:44395/api/Account/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "test@example.com",
    "password": "password123"
  }'
```

### Test Case 2: Refresh token
```bash
curl -X POST "https://localhost:44395/api/Account/RefreshToken" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck..."
  }'
```

### Test Case 3: Revoke token
```bash
curl -X POST "https://localhost:44395/api/Account/RevokeToken" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "token": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck..."
  }'
```
