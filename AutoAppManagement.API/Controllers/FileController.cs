using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Common.Ulti;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [Authorize]
    public class FileController : BaseController
    {
        private readonly IFileUlti _fileUlti;
        public FileController(IRestOutput res, IHttpContextAccessor httpContextAccessor, IFileUlti fileUlti) : base(res, httpContextAccessor)
        {
            _fileUlti = fileUlti;
        }

        [HttpGet]
        [Route("images/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var imageFileStream = _fileUlti.ReadFile(imageName);
            if (imageFileStream != null)
            {
                return File(imageFileStream, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("download")]
        public IActionResult DownloadFile(string fileUrl)
        {
            var fileStream = _fileUlti.ReadFile(fileUrl);
            if (fileStream != null)
            {
                var contentType = "application/octet-stream"; // Định dạng cho file bất kỳ
                var fileName = Path.GetFileName(fileUrl); // Lấy tên file từ URL
                return File(fileStream, contentType, fileName);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await _fileUlti.SaveFile(file);

            return Ok();
        }
    }
}