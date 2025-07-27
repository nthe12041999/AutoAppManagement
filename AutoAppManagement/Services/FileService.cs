using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IFileService : IBaseService
    {
        Task<byte[]> DownloadFile(string fileUrl);
        Task<byte[]> GetImage(string fileUrl);
    }

    public class FileService : BaseService, IFileService
    {
        public FileService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, config, httpContextAccessor)
        {

        }

        public async Task<byte[]> DownloadFile(string fileUrl)
        {
            return await RequestAuthenGetFile(FileApiUrlDef.DownloadFile(fileUrl));
        }

        public async Task<byte[]> GetImage(string fileUrl)
        {
            return await RequestAuthenGetFile(FileApiUrlDef.GetImage(fileUrl));
        }
    }
}
