using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AutoAppManagement.Service.Common.Ulti
{
    public interface IFileUlti
    {
        Task<string> SaveFile(IFormFile file);
        Task<string> SaveFile(string content, string fileName);
        Task<IEnumerable<string>> SaveFiles(Dictionary<string, string> files); // Thêm phương thức mới
        FileStream ReadFile(string fileName);
    }

    public class FileUlti : IFileUlti
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileUlti(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var uploadFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Files");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var filePath = Path.Combine(uploadFolder, file.FileName);

            if (!File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return file.FileName; // Trả về tên file đã lưu
        }

        public async Task<string> SaveFile(string content, string fileName)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Invalid content or file name.");
            }

            var uploadFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Files");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var filePath = Path.Combine(uploadFolder, fileName);

            await File.WriteAllTextAsync(filePath, content);

            return fileName; // Trả về tên file đã lưu
        }

        public async Task<IEnumerable<string>> SaveFiles(Dictionary<string, string> files)
        {
            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                var fileName = file.Key; // Tên file
                var content = file.Value; // Nội dung

                if (!string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(fileName))
                {
                    await SaveFile(content, fileName); // Gọi phương thức SaveFile để lưu
                    savedFileNames.Add(fileName); // Thêm tên file vào danh sách đã lưu
                }
            }

            return savedFileNames; // Trả về danh sách tên file đã lưu
        }

        public FileStream ReadFile(string fileName)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", fileName);
            if (File.Exists(filePath))
            {
                return File.OpenRead(filePath);
            }
            return null; // Trả về null nếu không tìm thấy file
        }
    }
}
