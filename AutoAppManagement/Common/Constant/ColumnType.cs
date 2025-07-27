namespace AutoAppManagement.WebApp.Common.Constant
{
    public class ColumnType
    {
        public static readonly string String = "string";
        public static readonly string Numeric = "numeric";
        public static readonly string Date = "date";
        public static readonly string DateTime = "dateTime";
        public static readonly string Currency = "currency";
        public static readonly string NumFmt = "num-fmt"; // Sắp xếp số có dấu phẩy ngăn cách
        public static readonly string TitleString = "title-string"; // Sắp xếp theo giá trị trong thuộc tính "title"
        public static readonly string HtmlNum = "html-num"; // Xử lý sắp xếp như số, dù có HTML bao quanh
        public static readonly string HtmlNumFmt = "html-num-fmt"; // Sắp xếp số có định dạng bên trong HTML
        public static readonly string Enum = "enum";
        public static readonly string Html = "html";
        public static readonly string Amount = "amount";
    }
}
