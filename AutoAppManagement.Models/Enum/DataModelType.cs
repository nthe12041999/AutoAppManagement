using System.ComponentModel;

namespace AutoAppManagement.Models.Enum
{
    public class DataModelType
    {
        public enum GenderType : short
        {
            [Description("Nam")]
            Male,
            [Description("Nữ")]
            Female,
            [Description("Khác")]
            Other,
        }

        public enum ImgInforState : short
        {
            [Description("Thêm")]
            Add,
            [Description("Xóa")]
            Delete,
            [Description("Không thực hiện gì")]
            DoNothing
        }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public enum DeviceType : short
        {
            [Description("Máy tính để bàn")]
            Desktop,
            [Description("Laptop")]
            Laptop,
            [Description("Điện thoại")]
            Mobile,
            [Description("Máy tính bảng")]
            Tablet,
            [Description("TV thông minh")]
            SmartTV,
            [Description("Thiết bị khác")]
            Other
        }

        /// <summary>
        /// Hệ điều hành
        /// </summary>
        public enum OperatingSystemType : short
        {
            [Description("Windows")]
            Windows,
            [Description("macOS")]
            MacOS,
            [Description("Linux")]
            Linux,
            [Description("Android")]
            Android,
            [Description("iOS")]
            iOS,
            [Description("iPadOS")]
            iPadOS,
            [Description("Chrome OS")]
            ChromeOS,
            [Description("Ubuntu")]
            Ubuntu,
            [Description("CentOS")]
            CentOS,
            [Description("Khác")]
            Other
        }

        /// <summary>
        /// Loại license
        /// </summary>
        public enum LicenseTypeEnum : short
        {
            [Description("Miễn phí")]
            Free,
            [Description("Dùng thử")]
            Trial,
            [Description("Chuyên nghiệp")]
            Professional,
            [Description("Doanh nghiệp")]
            Enterprise,
        }

        /// <summary>
        /// Loại thông báo
        /// </summary>
        public enum NotificationType : short
        {
            [Description("Thông tin")]
            Info,
            [Description("Cảnh báo")]
            Warning,
            [Description("Lỗi")]
            Error,
            [Description("Thành công")]
            Success,
            [Description("Khuyến mãi")]
            Promotion,
            [Description("Hệ thống")]
            System,
            [Description("Bảo trí")]
            Maintenance
        }

        /// <summary>
        /// Hành động quyền
        /// </summary>
        public enum PermissionAction : short
        {
            [Description("Xem")]
            View,
            [Description("Tạo")]
            Create,
            [Description("Chỉnh sửa")]
            Update,
            [Description("Xóa")]
            Delete,
            [Description("Phê duyệt")]
            Approve,
            [Description("Từ chối")]
            Reject,
            [Description("Quản lý")]
            Manage,
            [Description("Xuất dữ liệu")]
            Export,
            [Description("Nhập dữ liệu")]
            Import,
            [Description("Thực thi")]
            Execute,
            [Description("Khóa")]
            Lock
        }

        /// <summary>
        /// Loại tool
        /// </summary>
        public enum ToolType : short
        {
            [Description("AI")]
            AI,
            [Description("Xử lý hình ảnh")]
            ImageProcessing,
            [Description("Xử lý văn bản")]
            TextProcessing,
            [Description("Phân tích dữ liệu")]
            DataAnalysis,
            [Description("Tự động hóa")]
            Automation,
            [Description("Bảo mật")]
            Security,
            [Description("Mạng")]
            Network,
            [Description("Cơ sở dữ liệu")]
            Database,
            [Description("Web")]
            Web,
            [Description("Mobile")]
            Mobile,
            [Description("Tiện ích")]
            Utility,
            [Description("Khác")]
            Other
        }

        /// <summary>
        /// Trạng thái chung
        /// </summary>
        public enum StatusType : short
        {
            [Description("Hoạt động")]
            Active,
            [Description("Không hoạt động")]
            Inactive,
            [Description("Đang chờ")]
            Pending,
            [Description("Đã khóa")]
            Locked,
            [Description("Đã xóa")]
            Deleted,
            [Description("Đã hết hạn")]
            Expired,
            [Description("Đang bảo trì")]
            Maintenance,
            [Description("Đã phê duyệt")]
            Approved,
            [Description("Bị từ chối")]
            Rejected,
            [Description("Nháp")]
            Draft
        }

        public enum Gender : short
        {
            [Description("Nam")]
            Male,
            [Description("Nữ")]
            Femal,
            [Description("Khác")]
            Other
        }
    }
}
