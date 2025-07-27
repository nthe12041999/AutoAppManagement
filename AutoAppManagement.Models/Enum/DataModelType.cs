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
    }
}
