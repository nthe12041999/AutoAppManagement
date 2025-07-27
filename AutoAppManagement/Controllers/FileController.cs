using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    [Route("File")]
    [Authorize]
    public class FileController : BaseController
    {
        private readonly IFileService _fileService;
        public FileController(RestOutput res, IFileService fileService) : base(res)
        {
            _fileService = fileService;
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var fileData = await _fileService.DownloadFile(@$"Scan/{fileName}");
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
            var fileData = await _fileService.DownloadFile(@$"{fileName}");
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
            var fileData = await _fileService.GetImage(@$"{imageName}");
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
