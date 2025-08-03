# AutoAppManagement - Postman Collection

Collection Postman duy nhất để test tất cả API endpoints của hệ thống AutoAppManagement.

## File Collection

### **AutoAppManagement_Complete_Postman_Collection.json**

File collection duy nhất chứa tất cả API endpoints của hệ thống, được tổ chức theo các nhóm chức năng:

- **Authentication**: Đăng nhập cho Admin và Customer
- **Account Management**: Quản lý tài khoản
- **Device Management**: Quản lý thiết bị
- **License Management**: Quản lý license
- **Notification Management**: Quản lý thông báo
- **File Management**: Quản lý file

## Environment File

**AutoAppManagement_Environment.postman_environment.json**
File environment chứa tất cả biến cần thiết:

- `baseUrl`: URL của API server (mặc định: https://localhost:7001)
- `accessToken`: Token xác thực (tự động set sau khi đăng nhập)
- `accountId`: ID tài khoản
- `deviceId`: ID thiết bị
- `licenseKey`: Key license
- Các biến khác cho testing

## Hướng dẫn sử dụng

### 1. Import vào Postman

1. Mở Postman
2. Click **Import**
3. Chọn file **AutoAppManagement_Complete_Postman_Collection.json**
4. Import file environment **AutoAppManagement_Environment.postman_environment.json**

### 2. Thiết lập Environment

1. Chọn environment "AutoAppManagement Environment"
2. Cập nhật `baseUrl` nếu cần thiết
3. Cập nhật username/password nếu khác mặc định

### 3. Workflow Testing

#### Đối với Admin/Accountant:

1. **Admin Login**: Đăng nhập admin để lấy token
2. **Get User Info Generic**: Lấy thông tin user hiện tại
3. **Get All Accounts**: Lấy danh sách tất cả tài khoản

#### Đối với Customer:

1. **Customer Login with Device**: Đăng nhập customer với thông tin thiết bị
2. **Register New Device**: Đăng ký thiết bị mới
3. **Get Account Devices**: Xem danh sách thiết bị
4. **Check Account License**: Kiểm tra license
5. **Validate Access**: Kiểm tra quyền truy cập

#### Đối với Notification:

1. **Get Count Unread Notifications**: Lấy số thông báo chưa đọc
2. **Get Notifications by Range**: Lấy danh sách thông báo
3. **Mark Notification as Read**: Đánh dấu đã đọc

#### Đối với File:

1. **Upload File**: Upload file lên server
2. **Get Image**: Lấy hình ảnh
3. **Download File**: Tải xuống file

### 4. Auto-Authentication

Các request đăng nhập đã được cấu hình script tự động để:

- Lưu access token vào environment variable
- Lưu account ID và device ID
- Tự động sử dụng cho các request tiếp theo

### 5. Variables được sử dụng

- `{{baseUrl}}`: URL base của API
- `{{accessToken}}`: Token xác thực
- `{{accountId}}`: ID tài khoản
- `{{deviceId}}`: ID thiết bị
- `{{licenseKey}}`: Key license
- `{{notificationId}}`: ID thông báo
- `{{imageName}}`: Tên file hình ảnh
- `{{fileUrl}}`: URL file cần tải

## Lưu ý

- Đảm bảo API server đang chạy trước khi test
- Token sẽ tự động được set sau khi đăng nhập thành công
- Một số API yêu cầu role cụ thể (Admin/Customer)
- File upload sử dụng form-data, cần chọn file thực tế để test

## Troubleshooting

- Nếu gặp lỗi 401: Kiểm tra token có hợp lệ không
- Nếu gặp lỗi 403: Kiểm tra role của user có phù hợp không
- Nếu gặp lỗi 404: Kiểm tra URL và endpoint có đúng không
- Nếu gặp lỗi 500: Kiểm tra server logs để debug
