using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.ViewModel
{
    public class ImgInfor
    {
        public string Base64 { get; set; }
        public string ContentType { get; set; }
        public string SeoFilename { get; set; }
        public ImgInforState State { get; set; }
    }
}
