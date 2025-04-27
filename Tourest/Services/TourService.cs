using Tourest.Data.Repositories;
using Tourest.ViewModels.Category;
using Tourest.ViewModels.ItineraryDay;
using Tourest.ViewModels.Tour;
using Tourest.ViewModels.TourRating;
using Tourest.ViewModels.UserView;

namespace Tourest.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository; // Inject Repository Interface
        // private readonly IMapper _mapper; // Tùy chọn: Inject AutoMapper

        public TourService(ITourRepository tourRepository /*, IMapper mapper*/)
        {
            _tourRepository = tourRepository;
            // _mapper = mapper;
        }

        public async Task<List<TourListViewModel>> GetFeaturedToursForDisplayAsync(int count)
        {
            var featuredTours = await _tourRepository.GetFeaturedToursAsync(count);

            // Mapping sang TourListViewModel, tính toán rating giống hệt các phương thức khác
            var tourViewModels = featuredTours.Select(tour => new TourListViewModel
            {
                TourId = tour.TourID,
                Name = tour.Name,
                Destination = tour.Destination,
                DurationDays = tour.DurationDays,
                ChildPrice = tour.ChildPrice, // Hoặc AdultPrice
                ThumbnailImageUrl = tour.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                // Tính rating từ dữ liệu include
                AverageRating = (tour.TourRatings != null && tour.TourRatings.Any())
                              ? tour.TourRatings.Average(tr => tr.Rating.RatingValue)
                              : (decimal?)null,
                SumRating = (tour.TourRatings != null) ? tour.TourRatings.Count() : 0
            }).ToList();

            return tourViewModels;
        }

        public async Task<IEnumerable<TourListViewModel>> GetActiveToursForDisplayAsync()
        {
            var activeTours = await _tourRepository.GetActiveToursAsync(); // Đã Include ratings

            var tourViewModels = activeTours.Select(tour => new TourListViewModel
            {
                TourId = tour.TourID,
                Name = tour.Name,
                Destination = tour.Destination,
                DurationDays = tour.DurationDays,
                ChildPrice = tour.ChildPrice,
                ThumbnailImageUrl = tour.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),

                // --- Tính toán Rating từ dữ liệu đã Include ---
                AverageRating = (tour.TourRatings != null && tour.TourRatings.Any())
                              ? tour.TourRatings.Average(tr => tr.Rating.RatingValue) // Tính trung bình
                              : (decimal?)null, // Trả về null nếu không có rating
                SumRating = (tour.TourRatings != null) ? tour.TourRatings.Count() : 0 // Đếm số lượng rating
                                                                                      // --- Kết thúc tính toán Rating ---

            }).ToList();

            return tourViewModels;
        }

        public async Task<TourDetailsViewModel?> GetTourDetailsAsync(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id); // Repo đã Include đủ thứ cần

            if (tour == null)
            {
                return null; // Không tìm thấy tour
            }

            // Tính toán rating (nhất quán với cách làm ở list view)
            decimal? averageRating = (tour.TourRatings != null && tour.TourRatings.Any())
                                   ? tour.TourRatings.Average(tr => tr.Rating.RatingValue)
                                   : (decimal?)null;
            int sumRating = (tour.TourRatings != null) ? tour.TourRatings.Count() : 0;

            // Mapping từ Tour entity sang TourDetailsViewModel
            var viewModel = new TourDetailsViewModel
            {
                TourId = tour.TourID,
                Name = tour.Name,
                Destination = tour.Destination,
                Description = tour.Description,
                DurationDays = tour.DurationDays,
                DurationNights = tour.DurationNights,
                AdultPrice = tour.AdultPrice,
                ChildPrice = tour.ChildPrice,
                MinGroupSize = tour.MinGroupSize,
                MaxGroupSize = tour.MaxGroupSize,
                Status = tour.Status,
                AverageRating = averageRating, // Gán giá trị đã tính
                SumRating = sumRating,         // Gán số lượng đã đếm
                IsCancellable = tour.IsCancellable,
                CancellationPolicyDescription = tour.CancellationPolicyDescription,

                // Tách chuỗi, xử lý null hoặc rỗng
                DeparturePointsList = tour.DeparturePoints?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),
                IncludedServicesList = tour.IncludedServices?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),
                ExcludedServicesList = tour.ExcludedServices?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),
                ImageUrlList = tour.ImageUrls?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),

                // Map các collection liên quan
                ItineraryDays = tour.ItineraryDays?.Select(it => new ItineraryDayViewModel
                {
                    DayNumber = it.DayNumber,
                    Title = it.Title ?? string.Empty, // Xử lý null
                    Description = it.Description ?? string.Empty // Xử lý null
                }).ToList() ?? new List<ItineraryDayViewModel>(),

                Categories = tour.TourCategories?.Select(tc => new CategoryViewModel
                {
                    CategoryId = tc.Category?.CategoryID ?? 0, // Xử lý null
                    Name = tc.Category?.Name ?? string.Empty // Xử lý null
                }).ToList() ?? new List<CategoryViewModel>(),

                Ratings = tour.TourRatings?.Select(tr => new TourRatingViewModel
                {
                    RatingId = tr.RatingID,
                    RatingValue = tr.Rating?.RatingValue ?? 0, // Xử lý null
                    Comment = tr.Rating?.Comment,
                    RatingDate = tr.Rating?.RatingDate ?? DateTime.MinValue, // Xử lý null
                    Customer = tr.Rating?.Customer == null ? null : new UserViewModel // Map thông tin khách hàng
                    {
                        CustomerId = tr.Rating.CustomerID,
                        FullName = tr.Rating.Customer.FullName ?? tr.Rating.Customer.FullName, // Ví dụ lấy FullName hoặc UserName
                                                                                               // ProfilePictureUrl = tr.Rating.Customer.ProfilePictureUrl // Nếu có
                    }
                }).OrderByDescending(r => r.RatingDate).ToList() ?? new List<TourRatingViewModel>() // Sắp xếp đánh giá mới nhất lên đầu
            };

            return viewModel;
        }
        public async Task<IEnumerable<TourListViewModel>> GetActiveToursForDisplayAsync(
         IEnumerable<int>? categoryIds = null,
         IEnumerable<string>? destinations = null,
         IEnumerable<int>? ratings = null,
         string? sortBy = null,
         int? minPrice = null, // Nhận minPrice
         int? maxPrice = null,
         string? searchDestination = null,
  string? searchCategoryName = null,
  DateTime? searchDate = null, // Nhận nhưng chưa dùng để lọc ở Repo
  int? searchGuests = null) // Nhận maxPrice
        {
            // === SỬA LỖI: Truyền minPrice và maxPrice xuống Repository ===
            var tours = await _tourRepository.GetActiveToursAsync(
         categoryIds, destinations, ratings, sortBy, minPrice, maxPrice,
         // --- TRUYỀN THAM SỐ ---
         searchDestination, searchCategoryName, searchDate, searchGuests
      ); // Truyền đủ tham số
            // === KẾT THÚC SỬA LỖI ===

            // Mapping giữ nguyên logic tính toán rating tức thời
            var tourViewModels = tours.Select(tour => new TourListViewModel
            {
                TourId = tour.TourID,
                Name = tour.Name,
                Destination = tour.Destination,
                DurationDays = tour.DurationDays,
                ChildPrice = tour.ChildPrice, // Hoặc AdultPrice
                ThumbnailImageUrl = tour.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                AverageRating = (tour.TourRatings != null && tour.TourRatings.Any()) ? tour.TourRatings.Average(tr => tr.Rating.RatingValue) : (decimal?)null,
                SumRating = (tour.TourRatings != null) ? tour.TourRatings.Count() : 0
            }).ToList();

            return tourViewModels;
        }


        // Implement phương thức mới
        public async Task<IEnumerable<string>> GetDestinationsForFilterAsync()
        {
            return await _tourRepository.GetDistinctActiveDestinationsAsync();
        }

        public async Task<int> GetActiveTourCountAsync() { return await _tourRepository.GetActiveTourCountAsync(); }
        public async Task<int> GetDistinctDestinationCountAsync() { return await _tourRepository.GetDistinctDestinationCountAsync(); }
    }
}
