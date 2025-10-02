# Tool Version Management API Documentation

## Tổng quan

Tool Version Management API cho phép quản lý và kiểm tra phiên bản của các tool. API này được thiết kế để các ứng dụng bên thứ 3 có thể gọi vào để kiểm tra và tải xuống phiên bản mới nhất.

## Base URL

```
Development: https://localhost:44395/api/toolversion
Production: https://yourdomain.com/api/toolversion
```

## Public Endpoints (Không cần xác thực)

### 1. Lấy version hiện tại của tool

**Endpoint:** `GET /api/toolversion/current/{toolCode}`

**Parameters:**
- `toolCode` (string, required): Mã của tool cần kiểm tra
- `platform` (string, optional): Platform (Windows, MacOS, Linux, Android, iOS)

**Example Request:**
```http
GET /api/toolversion/current/AUTO_CLICKER?platform=Windows
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "toolCode": "AUTO_CLICKER",
    "toolName": "Auto Clicker Pro",
    "currentVersion": "2.5.0",
    "minimumVersion": "2.0.0",
    "description": "Professional auto clicking tool with advanced features",
    "downloadUrl": "https://download.example.com/autoclicker/v2.5.0/setup.exe",
    "releaseDate": "2024-01-15T00:00:00",
    "isActive": true,
    "isRequired": false,
    "platform": "Windows",
    "fileSize": 5242880,
    "fileSizeFormatted": "5 MB",
    "checksum": "SHA256:abc123def456..."
  }
}
```

### 2. Kiểm tra cập nhật

**Endpoint:** `POST /api/toolversion/check`

**Request Body:**
```json
{
  "toolCode": "AUTO_CLICKER",
  "currentVersion": "2.4.0",
  "platform": "Windows"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "updateAvailable": true,
    "updateRequired": false,
    "latestVersion": "2.5.0",
    "minimumVersion": "2.0.0",
    "downloadUrl": "https://download.example.com/autoclicker/v2.5.0/setup.exe",
    "releaseNotes": "Major update with new UI and performance improvements",
    "releaseDate": "2024-01-15T00:00:00",
    "fileSize": 5242880,
    "checksum": "SHA256:abc123def456...",
    "features": [
      "New modern UI",
      "Multi-threading support",
      "Custom scripts",
      "Hotkey customization"
    ],
    "bugFixes": [
      "Fixed memory leak issue",
      "Resolved crash on Windows 11",
      "Fixed coordinate detection bug"
    ],
    "message": "A new version is available."
  }
}
```

### 3. Kiểm tra cập nhật nhanh

**Endpoint:** `GET /api/toolversion/check-update/{toolCode}/{currentVersion}`

**Parameters:**
- `toolCode` (string, required): Mã tool
- `currentVersion` (string, required): Version hiện tại đang sử dụng
- `platform` (string, optional): Platform

**Example Request:**
```http
GET /api/toolversion/check-update/AUTO_CLICKER/2.4.0?platform=Windows
```

**Response:**
```json
{
  "success": true,
  "data": {
    "updateAvailable": true,
    "updateRequired": false,
    "latestVersion": "2.5.0",
    "downloadUrl": "https://download.example.com/autoclicker/v2.5.0/setup.exe",
    "message": "A new version is available."
  }
}
```

### 4. Lấy lịch sử version

**Endpoint:** `GET /api/toolversion/history/{toolCode}`

**Parameters:**
- `toolCode` (string, required): Mã tool
- `limit` (int, optional): Số lượng version muốn lấy (default: 10)

**Example Request:**
```http
GET /api/toolversion/history/AUTO_CLICKER?limit=5
```

### 5. Lấy tất cả version đang active

**Endpoint:** `GET /api/toolversion/all-active`

**Example Request:**
```http
GET /api/toolversion/all-active
```

### 6. Lấy versions theo platform

**Endpoint:** `GET /api/toolversion/platform/{platform}`

**Parameters:**
- `platform` (string, required): Platform (Windows, MacOS, Linux, Android, iOS)

**Example Request:**
```http
GET /api/toolversion/platform/Windows
```

## Admin Endpoints (Yêu cầu xác thực)

### 1. Tạo version mới

**Endpoint:** `POST /api/toolversion/create`

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Request Body:**
```json
{
  "toolCode": "NEW_TOOL",
  "toolName": "New Tool Name",
  "currentVersion": "1.0.0",
  "minimumVersion": "1.0.0",
  "description": "Description of the tool",
  "downloadUrl": "https://download.example.com/tool/v1.0.0/setup.exe",
  "releaseNotes": "Initial release",
  "releaseDate": "2024-01-20T00:00:00",
  "isActive": true,
  "isRequired": false,
  "platform": "Windows",
  "fileSize": 10485760,
  "checksum": "SHA256:...",
  "features": ["Feature 1", "Feature 2"],
  "bugFixes": ["Bug fix 1", "Bug fix 2"],
  "category": "Utility",
  "priority": 1
}
```

### 2. Cập nhật version

**Endpoint:** `PUT /api/toolversion/update`

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Request Body:**
```json
{
  "id": 1,
  "currentVersion": "2.5.1",
  "releaseNotes": "Updated release notes",
  "downloadUrl": "https://download.example.com/tool/v2.5.1/setup.exe"
}
```

### 3. Activate/Deactivate version

**Activate:** `POST /api/toolversion/activate/{id}`

**Deactivate:** `POST /api/toolversion/deactivate/{id}`

## Mã lỗi

- `200 OK`: Thành công
- `400 Bad Request`: Dữ liệu không hợp lệ
- `401 Unauthorized`: Không có quyền truy cập
- `404 Not Found`: Không tìm thấy version
- `500 Internal Server Error`: Lỗi server

## Sample Integration Code

### C# Example

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class VersionChecker
{
    private readonly HttpClient _httpClient;
    private const string API_BASE_URL = "https://yourdomain.com/api/toolversion";

    public VersionChecker()
    {
        _httpClient = new HttpClient();
    }

    public async Task<bool> CheckForUpdatesAsync(string toolCode, string currentVersion)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{API_BASE_URL}/check-update/{toolCode}/{currentVersion}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);
                
                if (result.success == true && result.data.updateAvailable == true)
                {
                    Console.WriteLine($"New version available: {result.data.latestVersion}");
                    Console.WriteLine($"Download URL: {result.data.downloadUrl}");
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking for updates: {ex.Message}");
            return false;
        }
    }
}
```

### Python Example

```python
import requests

class VersionChecker:
    def __init__(self, base_url="https://yourdomain.com/api/toolversion"):
        self.base_url = base_url
    
    def check_for_updates(self, tool_code, current_version, platform=None):
        try:
            # Build URL
            url = f"{self.base_url}/check-update/{tool_code}/{current_version}"
            params = {"platform": platform} if platform else {}
            
            # Make request
            response = requests.get(url, params=params)
            
            if response.status_code == 200:
                data = response.json()
                if data["success"] and data["data"]["updateAvailable"]:
                    print(f"New version available: {data['data']['latestVersion']}")
                    print(f"Download URL: {data['data']['downloadUrl']}")
                    return True
            return False
            
        except Exception as e:
            print(f"Error checking for updates: {e}")
            return False

# Usage
checker = VersionChecker()
has_update = checker.check_for_updates("AUTO_CLICKER", "2.4.0", "Windows")
```

### JavaScript Example

```javascript
class VersionChecker {
    constructor(baseUrl = 'https://yourdomain.com/api/toolversion') {
        this.baseUrl = baseUrl;
    }

    async checkForUpdates(toolCode, currentVersion, platform = null) {
        try {
            let url = `${this.baseUrl}/check-update/${toolCode}/${currentVersion}`;
            if (platform) {
                url += `?platform=${platform}`;
            }

            const response = await fetch(url);
            const data = await response.json();

            if (data.success && data.data.updateAvailable) {
                console.log(`New version available: ${data.data.latestVersion}`);
                console.log(`Download URL: ${data.data.downloadUrl}`);
                return true;
            }
            return false;
        } catch (error) {
            console.error('Error checking for updates:', error);
            return false;
        }
    }
}

// Usage
const checker = new VersionChecker();
checker.checkForUpdates('AUTO_CLICKER', '2.4.0', 'Windows');
```

## Caching

API sử dụng cache với thời gian 30 phút cho các endpoint public để tối ưu hiệu suất. Cache sẽ tự động được làm mới khi có version mới được cập nhật.

## Rate Limiting

API áp dụng rate limiting:
- Public endpoints: 1000 requests/phút/IP
- Admin endpoints: 100 requests/phút/user

## Security Notes

1. Các endpoint admin yêu cầu JWT token trong header Authorization
2. Checksum được cung cấp để verify tính toàn vẹn của file download
3. HTTPS được khuyến nghị cho production environment
4. API keys có thể được implement thêm cho các partner cụ thể

## Contact

Nếu có vấn đề hoặc cần hỗ trợ, vui lòng liên hệ:
- Email: support@yourdomain.com
- Documentation: https://docs.yourdomain.com

