namespace Tourest.Services
{
    public class PhotoUploadResult
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public interface IPhotoService
    {
        // Upload ảnh, trả về PublicId và Url
        Task<PhotoUploadResult> UploadPhotoAsync(IFormFile file, string folderName, string? desiredPublicId = null);

        // Xóa ảnh dựa trên PublicId
        Task<bool> DeletePhotoAsync(string publicId);
    }
}
