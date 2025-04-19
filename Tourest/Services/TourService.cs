using Tourest.Data.Repositories;
using Tourest.ViewModels.Tour;

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

        public async Task<IEnumerable<TourListViewModel>> GetActiveToursForDisplayAsync()
        {
            // 1. Gọi Repository lấy Entities
            var activeTours = await _tourRepository.GetActiveToursAsync();

            // 2. Logic nghiệp vụ (nếu có) - Ví dụ: có thể lọc thêm theo mùa, khuyến mãi...
            // Hiện tại chưa có logic phức tạp cho ví dụ này

            // 3. Ánh xạ (Map) từ List<Tour> sang List<TourItemViewModel>
            var tourViewModels = activeTours.Select(tour => new TourListViewModel
            {
                TourId = tour.TourID,
                Name = tour.Name,
                Destination = tour.Destination,
                DurationDays = tour.DurationDays,
                ChildPrice = tour.ChildPrice, // Kiểu INT
                // Lấy ảnh đầu tiên làm thumbnail (ví dụ đơn giản)
                ThumbnailImageUrl = tour.ImageUrls?.Split(';').FirstOrDefault(url => !string.IsNullOrWhiteSpace(url)),
                AverageRating = tour.AverageRating

            }).ToList();

            // // Cách dùng AutoMapper (nếu đã cấu hình)
            // var tourViewModels = _mapper.Map<List<TourItemViewModel>>(activeTours);

            // 4. Trả về danh sách ViewModel
            return tourViewModels;
        }
    }
}
