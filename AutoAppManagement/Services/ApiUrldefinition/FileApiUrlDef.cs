namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class FileApiUrlDef
    {
        private const string pathController = "/api/File";

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetImage(string imageName)
        {
            return @$"{pathController}/images/{imageName}";
        }

        public static string DownloadFile(string fileUrl)
        {
            var encodedFileUrl = Uri.EscapeDataString(fileUrl);
            return @$"{pathController}/download?fileUrl={fileUrl}";
        }
    }
}
