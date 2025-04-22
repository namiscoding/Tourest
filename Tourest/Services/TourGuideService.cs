using Tourest.Data.Repositories;
using Tourest.ViewModels.TourGuide;
using Tourest.ViewModels.UserView;

namespace Tourest.Services
{
    public class TourGuideService : ITourGuideService // Hoặc UserService : IUserService
    {
        private readonly IUserRepository _userRepository; // Hoặc ITourGuideRepository
        // Inject các repo khác nếu cần

        public TourGuideService(IUserRepository userRepository) // Hoặc ITourGuideRepository
        {
            _userRepository = userRepository;
        }

        // ... các phương thức khác ...

        public async Task<TourGuideDetailsViewModel?> GetTourGuideDetailsAsync(int userId)
        {
            var user = await _userRepository.GetGuideByIdAsync(userId); // Gọi Repo

            if (user?.TourGuide == null) // Kiểm tra user và có phải là TourGuide không
            {
                return null;
            }

            // Mapping sang ViewModel
            var viewModel = new TourGuideDetailsViewModel
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl ?? "/assets/images/default-avatar.png", // Cung cấp ảnh default
                ExperienceLevel = user.TourGuide.ExperienceLevel,
                AverageRating = user.TourGuide.AverageRating, // Lấy từ cột đã lưu trữ
                RatingCount = user.TourGuideRatingsReceived?.Count ?? 0,

                // Tách chuỗi, giả sử phân tách bằng dấu phẩy (,) hoặc chấm phẩy (;)
                LanguagesSpokenList = user.TourGuide.LanguagesSpoken?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),
                SpecializationsList = user.TourGuide.Specializations?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),

                // Map danh sách đánh giá
                CustomerRatings = user.TourGuideRatingsReceived?
                    .Where(tgr => tgr.Rating != null) // Đảm bảo Rating tồn tại
                    .Select(tgr => new TourGuideRatingViewModel
                    {
                        RatingValue = tgr.Rating.RatingValue,
                        Comment = tgr.Rating.Comment,
                        RatingDate = tgr.Rating.RatingDate,
                        Customer = tgr.Rating.Customer == null ? null : new UserViewModel
                        {
                            CustomerId = tgr.Rating.CustomerID,
                            FullName = tgr.Rating.Customer.FullName ?? tgr.Rating.Customer.FullName ?? "Anonymous", // Ưu tiên FullName -> UserName -> Anonymous
                            ProfilePictureUrl = tgr.Rating.Customer.ProfilePictureUrl
                        }
                        // Map thêm thông tin TourGroup nếu cần: TourGroupName = tgr.TourGroup?.GroupName
                    })
                    .OrderByDescending(r => r.RatingDate) // Sắp xếp đánh giá mới nhất lên đầu
                    .ToList() ?? new List<TourGuideRatingViewModel>()
            };

            // Nếu AverageRating trong DB là null, thử tính toán tức thời (tùy chọn)
            if (!viewModel.AverageRating.HasValue && viewModel.CustomerRatings.Any())
            {
                viewModel.AverageRating = viewModel.CustomerRatings.Average(r => r.RatingValue);
                // Lưu ý: Việc tính toán ở đây chỉ để hiển thị, không cập nhật vào DB
                // Nên có cơ chế cập nhật TourGuide.AverageRating khi có đánh giá mới.
            }


            return viewModel;
        }
    }
}
