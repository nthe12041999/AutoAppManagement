using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.ViewModel;
using static AutoAppManagement.Models.Enum.DataModelType;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AutoAppManagement.WebApp.Services.Base
{
    public interface IBaseService
    {
        Task<T> RequestPostAsync<T>(string url, object model = null);
        Task<ResponseOutput<T>> RequestFullPostAsync<T>(string url, object model = null);
        Task<T> RequestAuthenPostAsync<T>(string url, object model = null);
        Task<ResponseOutput<T>> RequestFullAuthenPostAsync<T>(string url, object model = null);
        Task<T> RequestAuthenGetAsync<T>(string url);
        Task<ResponseOutput<T>> RequestFileAsync<T>(string url, List<IFormFile> selectedFile = null, object model = null);
        Task<ResponseOutput<T>> RequestFileByteAsync<T>(string url, List<IFormFile> selectedFile = null, object model = null);
        Task<List<ImgInfor>> ConvertFileToBase64(List<IFormFile> listSelectedFile);

        Task<object> RequestPostAsync(string url, object model = null);
        Task<RestOutput> RequestFullPostAsync(string url, object model = null);
        Task<object> RequestAuthenPostAsync(string url, object model = null);
        Task<RestOutput> RequestFullAuthenPostAsync(string url, object model = null);
    }

    public class BaseService : IBaseService
    {
        private readonly string _remoteServiceBaseUrl;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _remoteServiceBaseUrl = config.GetSection("BaseUrlApi").Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T> RequestPostAsync<T>(string url, object model = null)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync<T>(httpClient, url, model, null);
                    if (responseObject != null && responseObject.IsSuccess)
                    {
                        return responseObject.Data ?? default(T);
                    }

                    return default;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseOutput<T>> RequestFullPostAsync<T>(string url, object model = null)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync<T>(httpClient, url, model, null);

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> RequestAuthenPostAsync<T>(string url, object model = null)
        {
            try
            {
                var accessToken = GetBearerToken();
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync<T>(httpClient, url, model, accessToken);
                    if (responseObject != null && responseObject.IsSuccess)
                    {
                        return responseObject.Data ?? default(T);
                    }

                    return default;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseOutput<T>> RequestFullAuthenPostAsync<T>(string url, object model = null)
        {
            try
            {
                var accessToken = GetBearerToken();
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync<T>(httpClient, url, model, accessToken);

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> RequestPostAsync(string url, object model = null)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync(httpClient, url, model, null);
                    if (responseObject != null && responseObject.IsSuccess)
                    {
                        return responseObject.Data;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RestOutput> RequestFullPostAsync(string url, object model = null)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync(httpClient, url, model, null);

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> RequestAuthenPostAsync(string url, object model = null)
        {
            try
            {
                var accessToken = GetBearerToken();
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync(httpClient, url, model, accessToken);
                    if (responseObject != null && responseObject.IsSuccess)
                    {
                        return responseObject.Data;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RestOutput> RequestFullAuthenPostAsync(string url, object model = null)
        {
            try
            {
                var accessToken = GetBearerToken();
                using (var httpClient = _httpClientFactory.CreateClient())
                {

                    var responseObject = await PostAsync(httpClient, url, model, accessToken);

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<RestOutput> PostAsync(HttpClient httpClient, string url, object model = null, string accessToken = null)
        {
            HttpContent content = null;
            if (model != null)
            {
                // Kiểm tra nếu model có chứa IFormFile
                var modelType = model.GetType();
                var hasFile = modelType.GetProperties().Any(p => typeof(IFormFile).IsAssignableFrom(p.PropertyType) || typeof(IEnumerable<IFormFile>).IsAssignableFrom(p.PropertyType));

                if (hasFile)
                {
                    // Sử dụng MultipartFormDataContent nếu có IFormFile
                    var formData = new MultipartFormDataContent();

                    foreach (var prop in modelType.GetProperties())
                    {
                        var propValue = prop.GetValue(model);
                        if (propValue != null)
                        {
                            if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                            {
                                var file = (IFormFile)propValue;
                                var streamContent = new StreamContent(file.OpenReadStream());
                                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                formData.Add(streamContent, prop.Name, file.FileName);
                            }
                            else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(prop.PropertyType))
                            {
                                var files = (IEnumerable<IFormFile>)propValue;
                                foreach (var file in files)
                                {
                                    var streamContent = new StreamContent(file.OpenReadStream());
                                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                    formData.Add(streamContent, prop.Name, file.FileName);
                                }
                            }
                            else
                            {
                                // Các thuộc tính khác không phải là file
                                if (propValue is DateTime dateTimeValue)
                                {
                                    formData.Add(new StringContent(dateTimeValue.ToString("o")), prop.Name);
                                }
                                else
                                {
                                    formData.Add(new StringContent(propValue?.ToString() ?? string.Empty), prop.Name);
                                }
                            }
                        }
                    }

                    content = formData;
                }
                else
                {
                    // Nếu không có IFormFile, sử dụng JSON content
                    var jsonContent = JsonConvert.SerializeObject(model);
                    content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
            }

            // nếu có accessToken thì mới đưa Bearer Token vào
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            // Thực hiện cuộc gọi API POST
            var response = await httpClient.PostAsync(_remoteServiceBaseUrl + url, content);

            // Kiểm tra xem cuộc gọi API có thành công không
            response.EnsureSuccessStatusCode();

            // Đọc nội dung phản hồi
            var responseStr = await response.Content.ReadAsStringAsync();

            // Giải mã phản hồi JSON
            var responseObject = JsonConvert.DeserializeObject<RestOutput>(responseStr);

            return responseObject;
        }

        public async Task<T> RequestAuthenGetAsync<T>(string url)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var accessToken = GetBearerToken();
                    // nếu có accessToken thì mới đưa Bearer Token vào
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }

                    var response = await httpClient.GetAsync(_remoteServiceBaseUrl + url);

                    // Kiểm tra xem cuộc gọi API có thành công không
                    response.EnsureSuccessStatusCode();

                    // Đọc nội dung phản hồi
                    var responseStr = await response.Content.ReadAsStringAsync();

                    // Giải mã phản hồi JSON
                    var responseObject = JsonConvert.DeserializeObject<ResponseOutput<T>>(responseStr);
                    if (responseObject != null && responseObject.IsSuccess)
                    {
                        return responseObject.Data ?? default(T);
                    }

                    return default;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<byte[]> RequestAuthenGetFile(string url)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/octet-stream"); // Đặt kiểu dữ liệu cho tệp

                    var accessToken = GetBearerToken();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }

                    var response = await httpClient.GetAsync(_remoteServiceBaseUrl + url);
                    response.EnsureSuccessStatusCode();

                    // Đọc nội dung dưới dạng byte array
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseOutput<T>> PostAsync<T>(HttpClient httpClient, string url, object model = null, string accessToken = null)
        {
            HttpContent content = null;
            if (model != null)
            {
                // Kiểm tra nếu model có chứa IFormFile
                var modelType = model.GetType();
                var hasFile = modelType.GetProperties().Any(p => typeof(IFormFile).IsAssignableFrom(p.PropertyType) || typeof(IEnumerable<IFormFile>).IsAssignableFrom(p.PropertyType));

                if (hasFile)
                {
                    // Sử dụng MultipartFormDataContent nếu có IFormFile
                    var formData = new MultipartFormDataContent();

                    foreach (var prop in modelType.GetProperties())
                    {
                        var propValue = prop.GetValue(model);
                        if (propValue != null)
                        {
                            if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                            {
                                var file = (IFormFile)propValue;
                                var streamContent = new StreamContent(file.OpenReadStream());
                                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                formData.Add(streamContent, prop.Name, file.FileName);
                            }
                            else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(prop.PropertyType))
                            {
                                var files = (IEnumerable<IFormFile>)propValue;
                                foreach (var file in files)
                                {
                                    var streamContent = new StreamContent(file.OpenReadStream());
                                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                    formData.Add(streamContent, prop.Name, file.FileName);
                                }
                            }
                            else
                            {
                                // Các thuộc tính khác không phải là file
                                formData.Add(new StringContent(propValue.ToString()), prop.Name);
                            }
                        }
                    }

                    content = formData;
                }
                else
                {
                    // Nếu không có IFormFile, sử dụng JSON content
                    var jsonContent = JsonConvert.SerializeObject(model);
                    content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
            }

            // nếu có accessToken thì mới đưa Bearer Token vào
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            // Thực hiện cuộc gọi API POST
            var response = await httpClient.PostAsync(_remoteServiceBaseUrl + url, content);

            // Kiểm tra xem cuộc gọi API có thành công không
            response.EnsureSuccessStatusCode();

            // Đọc nội dung phản hồi
            var responseStr = await response.Content.ReadAsStringAsync();

            // Giải mã phản hồi JSON
            var responseObject = JsonConvert.DeserializeObject<ResponseOutput<T>>(responseStr);

            return responseObject;
        }

        public async Task<ResponseOutput<T>> RequestFileAsync<T>(string url, List<IFormFile> selectedFile = null, object model = null)
        {
            using (var httpClient = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    if (model != null)
                    {
                        var jsonContent = JsonConvert.SerializeObject(model);
                        formData.Add(new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                    }
                    // Thêm ảnh vào form data
                    if (selectedFile != null && selectedFile.Count > 0)
                    {
                        foreach (var item in selectedFile)
                        {
                            formData.Add(new StreamContent(item.OpenReadStream()), "file", item.Name);
                        }
                    }

                    var accessToken = GetBearerToken();
                    // nếu có accessToken thì mới đưa Bearer Token vào
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                    var response = await httpClient.PostAsync(_remoteServiceBaseUrl + url, formData);

                    // Kiểm tra xem cuộc gọi API có thành công không
                    response.EnsureSuccessStatusCode();

                    // Đọc nội dung phản hồi
                    var responseStr = await response.Content.ReadAsStringAsync();

                    // Giải mã phản hồi JSON
                    var responseObject = JsonConvert.DeserializeObject<ResponseOutput<T>>(responseStr);

                    return responseObject;
                }
            }
        }

        public async Task<ResponseOutput<T>> RequestFileByteAsync<T>(string url, List<IFormFile> selectedFile = null, object model = null)
        {
            using (var httpClient = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    if (model != null)
                    {
                        var jsonContent = JsonConvert.SerializeObject(model);
                        //formData.Add(new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                    }

                    foreach (var file in selectedFile)
                    {
                        // Đọc dữ liệu từ IFormFile và thêm nó vào multipart content
                        byte[] fileBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.OpenReadStream().CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }

                        var content = new ByteArrayContent(fileBytes);
                        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        formData.Add(content, "files", file.Name);
                    }


                    var accessToken = GetBearerToken();
                    // nếu có accessToken thì mới đưa Bearer Token vào
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                    var response = await httpClient.PostAsync(_remoteServiceBaseUrl + url, formData);

                    // Kiểm tra xem cuộc gọi API có thành công không
                    response.EnsureSuccessStatusCode();

                    // Đọc nội dung phản hồi
                    var responseStr = await response.Content.ReadAsStringAsync();

                    // Giải mã phản hồi JSON
                    var responseObject = JsonConvert.DeserializeObject<ResponseOutput<T>>(responseStr);

                    return responseObject;
                }
            }
        }

        public async Task<List<ImgInfor>> ConvertFileToBase64(List<IFormFile> listSelectedFile)
        {
            var lstContent = new List<ImgInfor>();
            foreach (var file in listSelectedFile)
            {
                if (file != null)
                {
                    using MemoryStream ms = new();
                    await file.OpenReadStream().CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    lstContent.Add(new ImgInfor
                    {
                        Base64 = Convert.ToBase64String(fileBytes),
                        ContentType = file.ContentType,
                        SeoFilename = file.Name
                    });
                }
            }
            return lstContent;
        }

        public List<ImgInfor> ConvertPathFileToImgInfor(List<string> listFilePath)
        {
            var lstContent = new List<ImgInfor>();
            foreach (var file in listFilePath)
            {
                if (file != null)
                {
                    lstContent.Add(new ImgInfor
                    {
                        SeoFilename = file,
                        State = ImgInforState.DoNothing
                    });
                }
            }
            return lstContent;
        }

        #region Đọc ghi localStorage

        protected string GetBearerToken()
        {
            var context = _httpContextAccessor.HttpContext;

            // Kiểm tra HttpContext và User
            if (context?.User != null)
            {
                // Lấy Claim chứa token
                var tokenClaim = context.User.Claims
                    .FirstOrDefault(c => c.Type == JwtRegisteredClaimsNamesConstant.Token);

                if (tokenClaim != null)
                {
                    return tokenClaim.Value;
                }
            }
            return null; // Trường hợp không có token
        }
        #endregion
    }
}
