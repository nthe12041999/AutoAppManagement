---
type: "always_apply"
description: "Example description"
---

Luôn trả lời bằng tiếng việt

- Chạy: `dotnet run` trong thư mục AutoAppManagement/AutoAppManagement
- Server chạy trên: http://127.0.0.1:5000 hoặc http://localhost:5000
- Kiểm tra port bằng: `netstat -ano | findstr :5000`

## Grid Pattern Standard

Khi tạo view có grid/table, sử dụng pattern từ `Views/Demo/Grid.cshtml`:

### Key Rules:

1. **Always use Grid Filter component** với data-component="card-filter"
2. **Unique container-id** cho mỗi filter
3. **Hidden result display** by default (style="display: none;")
4. **Bootstrap card structure** cho table
5. **Event-driven filtering** với custom events
6. **Responsive table** với table-responsive wrapper

### Implementation Steps:

1. **Copy Grid.cshtml structure** - Sử dụng làm template
2. **Update filter attributes** - Thay đổi data-container-id và options
3. **Customize table columns** - Thay đổi thead và tbody
4. **Update JavaScript** - Thay đổi event listener name
5. **Add action buttons** - Customize header buttons và table actions
