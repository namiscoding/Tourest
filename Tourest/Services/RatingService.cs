// File: Services/RatingService.cs
using Tourest.Data; // Namespace DbContext
using Tourest.Data.Entities;
using Tourest.Data.Repositories; // Namespace Repo Interfaces
using Tourest.ViewModels.TourRating; // Namespace CreateTourRatingViewModel
using Microsoft.EntityFrameworkCore; // Cho Transaction, Include và các phương thức LINQ Async
using Microsoft.AspNetCore.SignalR; // Cho SignalR Hub Context
using Tourest.Hubs; // Namespace của RatingHub
using Tourest.ViewModels.TourGuideRating;
namespace Tourest.Services
{
    public class RatingService : IRatingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRatingRepository _ratingRepository;
        private readonly ITourRatingRepository _tourRatingRepository;
        private readonly ITourRepository _tourRepository; 
        private readonly IHubContext<RatingHub> _ratingHubContext;
        private readonly ITourGuideRatingRepository _tourGuideRatingRepository;
        private readonly IUserRepository _userRepository;
        public RatingService(
            ApplicationDbContext context,
            IRatingRepository ratingRepository,
            ITourRatingRepository tourRatingRepository,
            ITourRepository tourRepository, // Vẫn inject nhưng có thể không dùng trực tiếp trong hàm này
            IHubContext<RatingHub> ratingHubContext, 
            ITourGuideRatingRepository tourGuideRatingRepository,
            IUserRepository userRepository)
        {
            _context = context;
            _ratingRepository = ratingRepository;
            _tourRatingRepository = tourRatingRepository;
            _tourRepository = tourRepository;
            _ratingHubContext = ratingHubContext;
            _tourGuideRatingRepository =  tourGuideRatingRepository;
            _userRepository = userRepository;
        }

        public async Task<int> GetTotalTourRatingCountAsync() { return await _ratingRepository.GetTotalTourRatingCountAsync(); }

        public async Task<(bool Success, string ErrorMessage)> AddTourRatingAsync(CreateTourRatingViewModel model, int customerId)
        {
            var tourExists = await _context.Tours.AnyAsync(t => t.TourID == model.TourId);
            if (!tourExists)
            {
                return (false, "Tour không tồn tại.");
            }

            // --- Bắt đầu transaction ---
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo và thêm Rating gốc qua Repository
                var rating = new Rating
                {
                    CustomerID = customerId,
                    RatingValue = model.RatingValue,
                    Comment = model.Comment,
                    RatingDate = DateTime.UtcNow,
                    RatingType = "Tour"
                };
                // AddAsync chỉ thêm vào context, chưa lưu vào DB
                var addedRating = await _ratingRepository.AddAsync(rating);

                // 2. Tạo và thêm TourRating liên kết qua Repository
                var tourRating = new TourRating
                {
                    Rating = addedRating, // Gán đối tượng Rating đã được context theo dõi
                    TourID = model.TourId
                };
                await _tourRatingRepository.AddAsync(tourRating);
                // Chưa SaveChanges

                // 3. Lưu Rating và TourRating vào DB để có thể tính toán lại Average
                // Hoặc có thể tính toán dựa trên dữ liệu đang theo dõi trong context nếu phức tạp hơn
                await _context.SaveChangesAsync(); // Lưu 2 bản ghi mới

                // === SỬA LỖI TÍNH TOÁN AVERAGE & COUNT ===
                // 4. Tính toán lại AverageRating VÀ RatingCount cho Tour này
                var tourRatingsQuery = _context.TourRatings
                                          .Where(tr => tr.TourID == model.TourId)
                                          .Include(tr => tr.Rating); // Include Rating để lấy RatingValue

                // Lấy danh sách các giá trị rating
                var ratingValues = await tourRatingsQuery
                                          .Select(tr => tr.Rating.RatingValue)
                                          .ToListAsync();

                // Tính toán từ danh sách đã lấy
                decimal? calculatedAverage = null; // Dùng nullable decimal
                int ratingCount = ratingValues.Count; // Đếm số lượng

                if (ratingCount > 0)
                {
                    calculatedAverage = ratingValues.Average(); // Tính trung bình từ danh sách
                }
                // === KẾT THÚC SỬA LỖI TÍNH TOÁN ===

                // 5. Cập nhật Tour entity
                var tourToUpdate = await _context.Tours.FindAsync(model.TourId);
                if (tourToUpdate != null)
                {
                    tourToUpdate.AverageRating = calculatedAverage; // Gán giá trị trung bình đã tính (có thể null)
                    // Không cần gọi _context.Tours.Update() vì FindAsync đã theo dõi
                }
                else
                {
                    await transaction.RollbackAsync();
                    // Log hoặc xử lý lỗi nghiêm trọng: Tour tồn tại ở bước kiểm tra đầu nhưng không tìm thấy ở đây?
                    return (false, "Lỗi không tìm thấy tour để cập nhật điểm đánh giá.");
                }

                // 6. Lưu thay đổi cho Tour (cập nhật AverageRating)
                await _context.SaveChangesAsync();

                // 7. Commit transaction
                await transaction.CommitAsync();

                // 8. Gửi tín hiệu SignalR (Sử dụng giá trị đã tính toán chính xác)
                await _ratingHubContext.Clients
                       .Group($"tour-{model.TourId}")
                       .SendAsync("UpdateTourRating", model.TourId, calculatedAverage, ratingCount); // Dùng calculatedAverage và ratingCount

                return (true, "Đánh giá của bạn đã được gửi thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback nếu có bất kỳ lỗi nào
                Console.WriteLine($"Error adding tour rating for TourId {model.TourId}: {ex.ToString()}"); // Log lỗi chi tiết hơn
                return (false, "Đã có lỗi hệ thống xảy ra khi gửi đánh giá. Vui lòng thử lại.");
            }
        }
        public async Task<(bool Success, string ErrorMessage)> AddTourGuideRatingAsync(CreateTourGuideRatingViewModel model, int customerId)
        {
            // Kiểm tra TourGuide và TourGroup tồn tại (an toàn hơn)
            var guideExists = await _context.Users.AnyAsync(u => u.UserID == model.TourGuideId && u.TourGuide != null);
            var groupExists = await _context.TourGroups.AnyAsync(tg => tg.TourGroupID == model.TourGroupId);
            if (!guideExists || !groupExists)
            {
                return (false, "Hướng dẫn viên hoặc nhóm tour không hợp lệ.");
            }

            // Tùy chọn: Kiểm tra xem user này đã đánh giá guide này trong group này chưa
            // bool alreadyRated = await _tourGuideRatingRepository.ExistsAsync(model.TourGroupId, customerId);
            // if (alreadyRated) return (false, "Bạn đã đánh giá hướng dẫn viên cho chuyến đi này rồi.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo Rating gốc
                var rating = new Rating
                {
                    CustomerID = customerId,
                    RatingValue = model.RatingValue,
                    Comment = model.Comment,
                    RatingDate = DateTime.UtcNow,
                    RatingType = "TourGuide" // Loại rating là TourGuide
                };
                var addedRating = await _ratingRepository.AddAsync(rating);
                // await _context.SaveChangesAsync(); // Lưu để lấy RatingID nếu cần ngay

                // 2. Tạo TourGuideRating liên kết
                var tourGuideRating = new TourGuideRating
                {
                    Rating = addedRating, // Hoặc RatingID = addedRating.RatingID nếu đã SaveChanges
                    TourGuideID = model.TourGuideId,
                    TourGroupID = model.TourGroupId
                };
                await _tourGuideRatingRepository.AddAsync(tourGuideRating);

                // 3. Lưu Rating và TourGuideRating
                await _context.SaveChangesAsync();

                // 4. Tính toán và cập nhật AverageRating cho TourGuide
                var guideRatings = await _context.TourGuideRatings
                                        .Where(tgr => tgr.TourGuideID == model.TourGuideId)
                                        .Include(tgr => tgr.Rating) // Include để lấy RatingValue
                                        .Select(tgr => tgr.Rating.RatingValue)
                                        .ToListAsync();

                decimal newAverage = guideRatings.Any() ? guideRatings.Average() : 0m;

                // Gọi Repo để cập nhật (Repo sẽ tự SaveChanges nếu cần hoặc chờ SaveChanges cuối)
                await _userRepository.UpdateTourGuideRatingAsync(model.TourGuideId, newAverage);
                // Lưu thay đổi cuối cùng (cập nhật TourGuide.AverageRating)
                await _context.SaveChangesAsync();


                // 5. Commit transaction
                await transaction.CommitAsync();
                return (true, "Đánh giá hướng dẫn viên thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error adding tour guide rating: {ex.ToString()}");
                return (false, "Lỗi hệ thống khi gửi đánh giá hướng dẫn viên.");
            }
        }
        // Các phương thức khác của IRatingService (nếu có)
    }
}