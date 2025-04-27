namespace Tourest.Helpers
{
    public static class CloudinaryHelper
    {
        // Lưu ý: Khởi tạo Cloudinary client mỗi lần gọi không hiệu quả.
        // Cách tốt hơn: Inject IConfiguration vào Service/Controller rồi truyền cloudName vào helper,
        // Hoặc tạo Cloudinary instance dạng Singleton và inject vào helper/service.
        // Ví dụ này làm đơn giản để minh họa cách tạo URL.

        // Hàm tạo URL cơ bản
        public static string GetImageUrl(string? publicId, string cloudName, string defaultImage = "/images/default-avatar.png")
        {
            if (string.IsNullOrEmpty(publicId) || string.IsNullOrEmpty(cloudName))
                return defaultImage;

            // Chỉ cần tạo URL đơn giản, không cần SDK nếu không transform phức tạp
            // Lưu ý: Luôn dùng HTTPS (res.cloudinary.com)
            return $"https://res.cloudinary.com/{cloudName}/image/upload/{publicId}";
        }

        // Hàm tạo URL có transformation (ví dụ thumbnail)
        public static string GetThumbnailUrl(string? publicId, int width, int height, string cloudName, string defaultImage = "/images/default-avatar.png")
        {
            if (string.IsNullOrEmpty(publicId) || string.IsNullOrEmpty(cloudName))
                return defaultImage;

            // Ví dụ transformation: width=150, height=150, crop=fill, gravity=face
            string transformation = $"w_{width},h_{height},c_fill,g_face";

            return $"https://res.cloudinary.com/{cloudName}/image/upload/{transformation}/{publicId}";
        }

        // --- Cách tốt hơn dùng SDK (nếu Cloudinary client đã được khởi tạo dạng Singleton hoặc Scoped) ---
        // public static string GetImageUrlWithSdk(string? publicId, Cloudinary cloudinaryInstance, Transformation transformation, string defaultImage = "/images/default-avatar.png")
        // {
        //     if (string.IsNullOrEmpty(publicId) || cloudinaryInstance == null)
        //          return defaultImage;
        //
        //     var url = cloudinaryInstance.Api.UrlImgUp
        //                 .Transform(transformation)
        //                 .Secure(true)
        //                 .BuildUrl(publicId);
        //     return url;
        // }
    }
}
