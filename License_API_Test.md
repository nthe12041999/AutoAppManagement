# License API Test

## 1. Đầu tiên cần login để lấy token:

```bash
curl --location 'https://localhost:7000/api/Auth/Login' \
--header 'Content-Type: application/json' \
--data-raw '{
  "username": "admin",
  "password": "123456",
  "rememberMe": true
}'
```

## 2. Tạo License mới (với token):

```bash
curl --location 'https://localhost:7000/api/License/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
  "licenseKey": "LIC-2024-NEW",
  "licenseName": "Pro License",
  "licenseType": "Pro",
  "description": "Professional license with full features",
  "accountId": 1,
  "maxDevices": 5,
  "maxUsers": 10,
  "startDate": "2024-01-01T00:00:00Z",
  "expiryDate": "2024-12-31T23:59:59Z",
  "price": 299.99,
  "currency": "USD",
  "isAutoRenewal": false,
  "allowedFeatures": "AI_TEXT_GEN,IMG_PROCESS,DATA_EXPORT",
  "usageLimits": "{\"dailyRequests\": 1000, \"monthlyQuota\": 30000}",
  "paymentInfo": "Credit Card Payment - Transaction ID: TXN123456",
  "state": 1
}'
```

## 3. Hoặc test với dữ liệu đơn giản hơn:

```bash
curl --location 'https://localhost:7000/api/License/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
  "licenseKey": "LIC-2024-BASIC",
  "licenseName": "Basic License",
  "licenseType": "Basic",
  "description": "Basic license for testing",
  "accountId": 1,
  "maxDevices": 1,
  "maxUsers": 1,
  "startDate": "2024-01-01T00:00:00.000Z",
  "expiryDate": "2024-12-31T23:59:59.000Z",
  "price": 99.99,
  "currency": "VND",
  "isAutoRenewal": false,
  "allowedFeatures": "BASIC_FEATURES",
  "state": 1
}'
```

## 4. Lấy danh sách License:

```bash
curl --location 'https://localhost:7000/api/License/GetPaging' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--header 'Content-Type: application/json' \
--data '{
  "pageIndex": 1,
  "pageSize": 10,
  "filter": "",
  "sort": "Id"
}'
```

## Các lưu ý:

1. **Port**: Dùng `7000` thay vì `44395`
2. **HTTPS**: Project chạy trên HTTPS
3. **Authentication**: Phải có Bearer token từ login
4. **Required Fields**: 
   - `licenseKey` (unique)
   - `licenseName` 
   - `licenseType`
   - `accountId`
   - `startDate`
   - `state` (1 = Add, 2 = Edit, 3 = Remove)

## EntityState values:
- `1` = Add (tạo mới)
- `2` = Edit (cập nhật) 
- `3` = Remove (xóa)
