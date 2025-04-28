using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Tourest.Data.Entities;

namespace Tourest.Services
{
    public class CloudinaryPhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryPhotoService> _logger;

        // Inject IConfiguration để đọc settings
        public CloudinaryPhotoService(IConfiguration config, ILogger<CloudinaryPhotoService> logger)
        {
            _logger = logger;
            // Đọc thông tin cấu hình (từ User Secrets hoặc appsettings)
            var cloudName = config["CloudinarySettings:CloudName"];
            var apiKey = config["CloudinarySettings:ApiKey"];
            var apiSecret = config["CloudinarySettings:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                _logger.LogError("Cloudinary account information is not configured correctly.");
                // Throw exception hoặc xử lý phù hợp để báo lỗi thiếu cấu hình
                throw new InvalidOperationException("Cloudinary configuration is missing in settings.");
            }

            CloudinaryDotNet.Account account = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Luôn dùng HTTPS
        }

        public async Task<PhotoUploadResult> UploadPhotoAsync(IFormFile file, string folderName, string? desiredPublicId = null)
        {
            var result = new PhotoUploadResult();
            if (file == null || file.Length <= 0)
            {
                result.ErrorMessage = "No file selected or file is empty.";
                return result; // Trả về lỗi nếu không có file
            }

            _logger.LogInformation("Attempting to upload file {FileName} to Cloudinary folder {FolderName}", file.FileName, folderName);

            await using var stream = file.OpenReadStream(); // Mở stream từ IFormFile

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                // Cấu hình biến đổi ảnh khi upload (ví dụ: resize ảnh profile)
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                // Đặt tên thư mục trên Cloudinary
                Folder = $"tourest/{folderName}", // Ví dụ: tourest/users hoặc tourest/tours
                // Đặt PublicId (nếu không đặt Cloudinary sẽ tự sinh)
                PublicId = desiredPublicId, // Nếu desiredPublicId là null, Cloudinary tự sinh
                Overwrite = true // Ghi đè nếu PublicId đã tồn tại
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    _logger.LogError("Cloudinary upload failed: {Error}", uploadResult.Error.Message);
                    result.ErrorMessage = uploadResult.Error.Message;
                }
                else if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("File uploaded successfully to Cloudinary. PublicId: {PublicId}, Url: {Url}", uploadResult.PublicId, uploadResult.SecureUrl.ToString());
                    result.Success = true;
                    result.PublicId = uploadResult.PublicId; // Trả về PublicId
                    result.Url = uploadResult.SecureUrl.ToString(); // Trả về URL HTTPS
                }
                else
                {
                    _logger.LogError("Cloudinary upload failed with status code: {StatusCode}", uploadResult.StatusCode);
                    result.ErrorMessage = $"Cloudinary upload failed (Status: {uploadResult.StatusCode})";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Cloudinary upload.");
                result.ErrorMessage = $"An unexpected error occurred during upload: {ex.Message}";
            }

            return result;
        }

        public async Task<bool> DeletePhotoAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return false;

            var deleteParams = new DeletionParams(publicId);
            try
            {
                var result = await _cloudinary.DestroyAsync(deleteParams);
                if (result.Result == "ok")
                {
                    _logger.LogInformation("Successfully deleted photo with PublicId: {PublicId}", publicId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to delete photo with PublicId: {PublicId}. Result: {Result}", publicId, result.Result);
                    return false; // Hoặc throw exception tùy logic
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Cloudinary deletion for PublicId: {PublicId}", publicId);
                return false;
            }
        }
    }
}
