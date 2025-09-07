# Login with Resources API Documentation

## Tổng quan

API `LoginWithResources` cho phép người dùng đăng nhập và nhận thông tin chi tiết về các tài nguyên có thể sử dụng dựa trên license của họ.

## Endpoint

```
POST /api/Account/LoginWithResources
```

## Request

### Request Body

```json
{
  "emailOrPhone": "user@example.com",
  "password": "password123"
}
```

### Request Model - LoginRequest

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| emailOrPhone | string | Yes | Email hoặc số điện thoại của user |
| password | string | Yes | Mật khẩu (6-100 ký tự) |

## Response

### Success Response

```json
{
  "isSuccess": true,
  "message": "Đăng nhập thành công",
  "data": {
    "account": {
      "id": 1,
      "userName": "user123",
      "email": "user@example.com",
      "name": "Nguyen Van A",
      "phone": "0123456789",
      // ... other account fields
    },
    "licenseInfo": {
      "licenseId": 1,
      "licenseKey": "LIC-2024-001",
      "licenseName": "Premium License",
      "licenseType": "Premium",
      "startDate": "2024-01-01T00:00:00Z",
      "endDate": "2025-01-01T00:00:00Z",
      "status": "Active",
      "daysRemaining": 180,
      "warningMessage": ""
    },
    "availableResources": [
      {
        "featureId": 1,
        "featureName": "AI Text Generation",
        "featureCode": "AI_TEXT_GEN",
        "toolName": "AI Tools",
        "description": "Generate text using AI",
        "isEnabled": true,
        "usageLimit": 1000,
        "usedCount": 150,
        "remainingCount": 850,
        "periodStart": "2024-01-01T00:00:00Z",
        "periodEnd": "2024-01-31T23:59:59Z",
        "limitType": "monthly",
        "status": "available",
        "warningMessage": ""
      },
      {
        "featureId": 2,
        "featureName": "Image Processing",
        "featureCode": "IMG_PROCESS",
        "toolName": "Image Tools",
        "description": "Process and edit images",
        "isEnabled": true,
        "usageLimit": 500,
        "usedCount": 495,
        "remainingCount": 5,
        "periodStart": "2024-01-01T00:00:00Z",
        "periodEnd": "2024-01-31T23:59:59Z",
        "limitType": "monthly",
        "status": "limited",
        "warningMessage": "Còn lại 5 lượt sử dụng"
      }
    ],
    "loginTime": "2024-01-15T10:30:00Z",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "tokenExpiry": "2024-01-15T18:30:00Z"
  }
}
```

### Error Response

```json
{
  "isSuccess": false,
  "message": "Tài khoản không tồn tại",
  "data": null
}
```

## Response Models

### LoginWithResourcesResponse

| Field | Type | Description |
|-------|------|-------------|
| account | AccountDTO | Thông tin tài khoản |
| licenseInfo | LicenseInfoDTO | Thông tin license |
| availableResources | ToolResourceDTO[] | Danh sách tài nguyên có thể sử dụng |
| loginTime | DateTime | Thời gian đăng nhập |
| message | string | Thông báo |
| token | string | JWT token |
| tokenExpiry | DateTime | Thời gian hết hạn token |

### ToolResourceDTO

| Field | Type | Description |
|-------|------|-------------|
| featureId | long | ID của feature |
| featureName | string | Tên feature |
| featureCode | string | Mã code của feature |
| toolName | string | Tên tool chứa feature |
| description | string | Mô tả feature |
| isEnabled | bool | Feature có được kích hoạt không |
| usageLimit | int? | Giới hạn sử dụng (null = unlimited) |
| usedCount | int | Số lượt đã sử dụng |
| remainingCount | int | Số lượt còn lại |
| periodStart | DateTime? | Ngày bắt đầu chu kỳ |
| periodEnd | DateTime? | Ngày kết thúc chu kỳ |
| limitType | string | Loại giới hạn (daily, monthly, total) |
| status | string | Trạng thái (available, limited, exhausted, disabled, not_started, expired) |
| warningMessage | string | Thông báo cảnh báo |

### Resource Status

| Status | Description |
|--------|-------------|
| available | Tài nguyên khả dụng |
| limited | Tài nguyên sắp hết (≤ 5 lượt) |
| exhausted | Đã hết lượt sử dụng |
| disabled | Tính năng bị vô hiệu hóa |
| not_started | Chưa đến thời gian sử dụng |
| expired | Đã hết hạn sử dụng |

## Các lỗi phổ biến

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "Tài khoản không tồn tại" | Email/phone không đúng | Kiểm tra lại thông tin đăng nhập |
| "Mật khẩu không chính xác" | Password sai | Nhập đúng password |
| "Tài khoản đã bị khóa" | Account bị lock | Liên hệ admin |
| "Tài khoản chưa được kích hoạt" | Account chưa active | Kích hoạt tài khoản |
| "Tài khoản đã hết hạn" | Account expired | Gia hạn tài khoản |
| "Không có license hợp lệ" | Không có license active | Kiểm tra license |

## Ví dụ sử dụng

### JavaScript/Fetch

```javascript
async function loginWithResources(emailOrPhone, password) {
  try {
    const response = await fetch('/api/Account/LoginWithResources', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        emailOrPhone: emailOrPhone,
        password: password
      })
    });
    
    const result = await response.json();
    
    if (result.isSuccess) {
      // Lưu token
      localStorage.setItem('token', result.data.token);
      
      // Hiển thị thông tin tài nguyên
      const resources = result.data.availableResources;
      resources.forEach(resource => {
        console.log(`${resource.featureName}: ${resource.remainingCount}/${resource.usageLimit || 'unlimited'}`);
        
        if (resource.status === 'limited') {
          console.warn(resource.warningMessage);
        }
      });
      
      return result.data;
    } else {
      throw new Error(result.message);
    }
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
}

// Sử dụng
loginWithResources('user@example.com', 'password123')
  .then(data => {
    console.log('Login successful:', data);
  })
  .catch(error => {
    console.error('Login error:', error);
  });
```

### C# HttpClient

```csharp
public async Task<LoginWithResourcesResponse> LoginWithResourcesAsync(string emailOrPhone, string password)
{
    var request = new LoginRequest
    {
        EmailOrPhone = emailOrPhone,
        Password = password
    };
    
    var response = await httpClient.PostAsJsonAsync("/api/Account/LoginWithResources", request);
    
    if (response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
        
        if (result.IsSuccess)
        {
            var loginData = JsonSerializer.Deserialize<LoginWithResourcesResponse>(result.Data.ToString());
            return loginData;
        }
        else
        {
            throw new Exception(result.Message);
        }
    }
    else
    {
        throw new Exception("API call failed");
    }
}
```

### cURL

```bash
curl -X POST "https://localhost:7000/api/Account/LoginWithResources" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "user@example.com",
    "password": "password123"
  }'
```

## Lưu ý quan trọng

1. **Token Security**: JWT token cần được lưu trữ an toàn và sử dụng trong Authorization header cho các API calls tiếp theo.

2. **Resource Monitoring**: Client nên theo dõi `remainingCount` và `status` để cảnh báo user khi sắp hết lượt sử dụng.

3. **Error Handling**: Luôn xử lý các error cases và hiển thị thông báo phù hợp cho user.

4. **Token Refresh**: Kiểm tra `tokenExpiry` và refresh token khi cần thiết.

5. **Feature Access**: Trước khi sử dụng một feature, check `isEnabled` và `status` để đảm bảo feature khả dụng.

## Integration với Frontend

```javascript
// Ví dụ xử lý resources sau khi login
function handleLoginResources(resources) {
  const resourceMap = {};
  
  resources.forEach(resource => {
    resourceMap[resource.featureCode] = resource;
    
    // Cập nhật UI cho từng feature
    const featureElement = document.getElementById(resource.featureCode);
    if (featureElement) {
      featureElement.dataset.enabled = resource.isEnabled;
      featureElement.dataset.remaining = resource.remainingCount;
      
      if (resource.status === 'limited') {
        featureElement.classList.add('warning');
      } else if (resource.status === 'exhausted') {
        featureElement.classList.add('disabled');
      }
    }
  });
  
  // Lưu resource info để sử dụng sau
  window.userResources = resourceMap;
}

// Function check trước khi sử dụng feature
function canUseFeature(featureCode) {
  const resource = window.userResources[featureCode];
  
  if (!resource) {
    return { canUse: false, reason: 'Feature not found' };
  }
  
  if (!resource.isEnabled) {
    return { canUse: false, reason: 'Feature disabled' };
  }
  
  if (resource.status === 'exhausted') {
    return { canUse: false, reason: 'Usage limit exceeded' };
  }
  
  if (resource.status === 'expired') {
    return { canUse: false, reason: 'Feature expired' };
  }
  
  return { canUse: true, remaining: resource.remainingCount };
}
```
