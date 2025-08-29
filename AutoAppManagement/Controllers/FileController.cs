using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    [Route("File")]
    //[Authorize]
    public class FileController : BaseController
    {
        protected IFileService _fileService;
        protected IFileService FileService
            => _fileService ??= _serviceProvider.GetRequiredService<IFileService>();
        public FileController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var fileData = await FileService.DownloadFile(@$"Scan/{fileName}");
            if (fileData != null)
            {
                return File(fileData, "application/octet-stream");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("downloadImg/{fileName}")]
        public async Task<IActionResult> DownloadFileImg(string fileName)
        {
            var fileData = await FileService.DownloadFile(@$"{fileName}");
            if (fileData != null)
            {
                return File(fileData, "application/octet-stream");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("images/{imageName}")]
        public async Task<IActionResult> GetImage(string imageName)
        {
            var fileData = await FileService.GetImage(@$"{imageName}");
            if (fileData != null)
            {
                return File(fileData, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
        }

    }
}
