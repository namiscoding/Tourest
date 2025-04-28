using System.Security.Claims;
using System.Text.Json;
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Helpers;
using Tourest.ViewModels.Admin.AdminTour;
using Tourest.ViewModels.Category;
using Tourest.ViewModels.ItineraryDay;
using Tourest.ViewModels.Tour;
using Tourest.ViewModels.TourRating;
using Tourest.ViewModels.UserView;

namespace Tourest.Services
{
    public class TourService : ITourService
    {
        private readonly ApplicationDbContext _context; 
        private readonly ITourRepository _tourRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPhotoService _photoService;
        private readonly ILogger<TourService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly string _cloudName; 


        // private readonly IMapper _mapper; // Nếu dùng AutoMapper

        public TourService(ApplicationDbContext context, ITourRepository tourRepository, ICategoryRepository categoryRepository, IPhotoService photoService, ILogger<TourService> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration /*, IMapper mapper*/)
        {
            _context = context;
            _tourRepository = tourRepository;
            _categoryRepository = categoryRepository;
            _photoService = photoService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _cloudName = configuration["CloudinarySettings:CloudName"] ?? ""; // Lấy CloudName
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
        private int GetCurrentUserId() // Helper lấy User ID
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdClaim, out int userId);
            return userId; // Trả về 0 nếu không lấy được
        }

        private string SerializeToJson(object? obj) // Helper Serialize cho Audit Log
        {
            if (obj == null) return "null";
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false }); // Không cần thụt lề
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing object to JSON for audit log.");
                return "{\"error\":\"serialization failed\"}";
            }
        }

        public async Task<PaginatedList<AdminTourListViewModel>> GetToursForAdminAsync(int pageIndex, int pageSize, string? searchTerm, string? statusFilter)
        {
            _logger.LogInformation("Fetching tours for admin...");
            var (tours, totalCount) = await _tourRepository.GetToursPagedAsync(pageIndex, pageSize, searchTerm, statusFilter);

            // Map sang ViewModel
            var viewModels = tours.Select(t => new AdminTourListViewModel
            {
                TourId = t.TourID,
                Name = t.Name,
                Destination = t.Destination,
                DurationDays = t.DurationDays,
                AdultPrice = t.AdultPrice,
                Status = t.Status,
                ThumbnailUrl = CloudinaryHelper.GetThumbnailUrl(t.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(), 100, 75, _cloudName) // Lấy ảnh đầu tiên làm thumb
            }).ToList();

            return new PaginatedList<AdminTourListViewModel>(viewModels, totalCount, pageIndex, pageSize);
        }

        public async Task<AdminTourDetailsViewModel?> GetTourDetailsForAdminAsync(int tourId)
        {
            _logger.LogInformation("Fetching tour details for admin. TourID: {TourId}", tourId);
            var tour = await _tourRepository.GetTourDetailsByIdAsync(tourId); // Repo lấy kèm Categories và Itinerary
            if (tour == null) return null;

            // Map sang ViewModel
            var viewModel = new AdminTourDetailsViewModel
            {
                TourID = tour.TourID,
                Name = tour.Name,
                Destination = tour.Destination,
                Description = tour.Description,
                DurationDays = tour.DurationDays,
                DurationNights = tour.DurationNights,
                AdultPrice = tour.AdultPrice,
                ChildPrice = tour.ChildPrice,
                MinGroupSize = tour.MinGroupSize,
                MaxGroupSize = tour.MaxGroupSize,
                DeparturePoints = tour.DeparturePoints,
                IncludedServices = tour.IncludedServices,
                ExcludedServices = tour.ExcludedServices,
                Status = tour.Status,
                AverageRating = tour.AverageRating,
                IsCancellable = tour.IsCancellable,
                CancellationPolicyDescription = tour.CancellationPolicyDescription,
                // Map Categories
                CategoryNames = tour.TourCategories?.Select(tc => tc.Category?.Name ?? "N/A").ToList() ?? new List<string>(),
                // Map Itinerary
                ItineraryDays = tour.ItineraryDays?.Select(id => new AdminItineraryDayViewModel
                {
                    ItineraryDayID = id.ItineraryDayID,
                    DayNumber = id.DayNumber,
                    Title = id.Title,
                    Description = id.Description,
                    Order = id.Order
                }).ToList() ?? new List<AdminItineraryDayViewModel>(),
                // Tạo danh sách URL ảnh từ PublicIds
                ImageUrls = tour.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(pid => CloudinaryHelper.GetImageUrl(pid, _cloudName, 800, 600)) // Lấy ảnh lớn hơn
                                   .ToList() ?? new List<string>()
            };  
            return viewModel;
        }

        public async Task<CreateEditTourViewModel?> GetTourForCreateAsync()
        {
            var allCategories = await _categoryRepository.GetAllAsync();
            var viewModel = new CreateEditTourViewModel
            {
                AvailableCategories = allCategories.Select(c => new CategorySelectionViewModel
                {
                    CategoryId = c.CategoryID,
                    Name = c.Name,
                    IsSelected = false // Mặc định chưa chọn
                }).ToList(),
                ItineraryDays = new List<AdminItineraryDayViewModel> { new AdminItineraryDayViewModel { DayNumber = 1, Order = 1 } } // Thêm sẵn 1 ngày
            };
            return viewModel;
        }

        public async Task<CreateEditTourViewModel?> GetTourForEditAsync(int tourId)
        {
            _logger.LogInformation("Fetching tour for edit. TourID: {TourId}", tourId);
            var tour = await _tourRepository.GetTourForEditByIdAsync(tourId); // Repo lấy kèm CategoryIds và ItineraryDays
            if (tour == null) return null;

            var allCategories = await _categoryRepository.GetAllAsync();
            var currentCategoryIds = tour.TourCategories?.Select(tc => tc.CategoryID).ToHashSet() ?? new HashSet<int>();
            var currentItinerary = tour.ItineraryDays?.Select(id => new AdminItineraryDayViewModel
            {
                ItineraryDayID = id.ItineraryDayID,
                DayNumber = id.DayNumber,
                Title = id.Title,
                Description = id.Description,
                Order = id.Order
            }).ToList() ?? new List<AdminItineraryDayViewModel>();
            var existingPublicIds = tour.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();


            // Map sang ViewModel
            var viewModel = new CreateEditTourViewModel
            {
                TourID = tour.TourID,
                Name = tour.Name,
                Destination = tour.Destination,
                Description = tour.Description,
                DurationDays = tour.DurationDays,
                DurationNights = tour.DurationNights,
                AdultPrice = tour.AdultPrice,
                ChildPrice = tour.ChildPrice,
                MinGroupSize = tour.MinGroupSize,
                MaxGroupSize = tour.MaxGroupSize,
                DeparturePoints = tour.DeparturePoints,
                IncludedServices = tour.IncludedServices,
                ExcludedServices = tour.ExcludedServices,
                Status = tour.Status,
                IsCancellable = tour.IsCancellable,
                CancellationPolicyDescription = tour.CancellationPolicyDescription,
                AvailableCategories = allCategories.Select(c => new CategorySelectionViewModel
                {
                    CategoryId = c.CategoryID,
                    Name = c.Name,
                    IsSelected = currentCategoryIds.Contains(c.CategoryID) // Đánh dấu category đã chọn
                }).ToList(),
                ItineraryDays = currentItinerary,
                ExistingImagePublicIds = existingPublicIds,
                // SelectedCategoryIds sẽ được binding từ form khi POST về
            };

            return viewModel;
        }

        public async Task<(bool Success, string ErrorMessage, int? CreatedTourId)> CreateTourAsync(CreateEditTourViewModel model, List<IFormFile>? imageFiles)
        {
            _logger.LogInformation("Attempting to create tour: {TourName}", model.Name);
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0) return (false, "Không thể xác định người dùng thực hiện.", null);

            // Map ViewModel to Tour Entity (basic info)
            var newTour = new Tour
            {
                Name = model.Name,
                Destination = model.Destination,
                Description = model.Description,
                DurationDays = model.DurationDays,
                DurationNights = model.DurationNights,
                AdultPrice = model.AdultPrice,
                ChildPrice = model.ChildPrice,
                MinGroupSize = model.MinGroupSize,
                MaxGroupSize = model.MaxGroupSize,
                DeparturePoints = model.DeparturePoints,
                IncludedServices = model.IncludedServices,
                ExcludedServices = model.ExcludedServices,
                Status = model.Status,
                IsCancellable = model.IsCancellable,
                CancellationPolicyDescription = model.CancellationPolicyDescription,
                // ImageUrls sẽ được cập nhật sau khi upload và có TourID
            };

            // Sử dụng Transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Thêm Tour cơ bản để lấy ID
                var createdTour = await _tourRepository.AddTourAsync(newTour);
                int newTourId = createdTour.TourID;
                _logger.LogInformation("Added base tour info with ID: {TourId}", newTourId);


                // 2. Xử lý Upload Ảnh Mới
                List<string> uploadedPublicIds = new List<string>();
                if (imageFiles != null && imageFiles.Any())
                {
                    _logger.LogInformation("Processing {Count} uploaded images for TourID: {TourId}", imageFiles.Count, newTourId);
                    int imageCounter = 1;
                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            string desiredPublicId = $"tours/{newTourId}/image_{imageCounter++}_{DateTime.UtcNow.Ticks}";
                            var uploadResult = await _photoService.UploadPhotoAsync(file, $"tours/{newTourId}", desiredPublicId);
                            if (uploadResult.Success && !string.IsNullOrEmpty(uploadResult.PublicId))
                            {
                                uploadedPublicIds.Add(uploadResult.PublicId);
                            }
                            else
                            {
                                _logger.LogWarning("Failed to upload image {FileName} for TourID {TourId}. Error: {Error}", file.FileName, newTourId, uploadResult.ErrorMessage);
                                // Có thể bỏ qua lỗi upload ảnh lẻ hoặc rollback transaction tùy yêu cầu
                            }
                        }
                    }
                }

                // 3. Cập nhật lại Tour với ImageUrls
                if (uploadedPublicIds.Any())
                {
                    createdTour.ImageUrls = string.Join(";", uploadedPublicIds);
                    await _tourRepository.UpdateTourAsync(createdTour); // Chỉ cập nhật trường ImageUrls
                    _logger.LogInformation("Updated Tour {TourId} with ImageUrls.", newTourId);
                }

                // 4. Cập nhật Categories
                if (model.SelectedCategoryIds != null)
                {
                    await _tourRepository.UpdateTourCategoriesAsync(newTourId, model.SelectedCategoryIds);
                    _logger.LogInformation("Updated categories for Tour {TourId}.", newTourId);
                }

                // 5. Cập nhật Itinerary
                if (model.ItineraryDays != null && model.ItineraryDays.Any())
                {
                    var itineraryEntities = model.ItineraryDays.Select(ivm => new ItineraryDay
                    {
                        TourID = newTourId, // Gán TourID
                        DayNumber = ivm.DayNumber,
                        Title = ivm.Title,
                        Description = ivm.Description,
                        Order = ivm.Order
                    }).ToList();
                    await _tourRepository.UpdateItineraryAsync(newTourId, itineraryEntities); // Repo sẽ xóa cũ thêm mới
                    _logger.LogInformation("Updated itinerary for Tour {TourId}.", newTourId);
                }

                // 6. Ghi Audit Log
                var auditLog = new TourAuditLog
                {
                    TourID = newTourId,
                    ActionType = "CREATE",
                    PerformedByUserID = currentUserId,
                    Timestamp = DateTime.UtcNow,
                    OldValues = null, // Không có giá trị cũ khi tạo mới
                    NewValues = SerializeToJson(createdTour) // Serialize Tour sau khi đã có ảnh
                };
                _context.TourAuditLogs.Add(auditLog); // Thêm vào context
                await _context.SaveChangesAsync(); // Lưu Audit Log
                _logger.LogInformation("Audit log created for Tour {TourId} CREATE action.", newTourId);


                // 7. Commit Transaction
                await transaction.CommitAsync();
                _logger.LogInformation("Tour {TourId} created successfully.", newTourId);
                return (true, string.Empty, newTourId);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating tour with name {TourName}. Transaction rolled back.", model.Name);
                // Cố gắng xóa ảnh đã upload nếu transaction lỗi? (Phức tạp)
                // foreach(var pid in uploadedPublicIds) { await _photoService.DeletePhotoAsync(pid); }
                return (false, "Đã có lỗi nghiêm trọng xảy ra khi tạo tour.", null);
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateTourAsync(CreateEditTourViewModel model, List<IFormFile>? newImageFiles, List<string>? imagesToDeletePublicIds)
        {
            _logger.LogInformation("Attempting to update tour with ID: {TourId}", model.TourID);
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0) return (false, "Không thể xác định người dùng thực hiện.");

            var existingTour = await _tourRepository.GetTourForEditByIdAsync(model.TourID); // Lấy kèm Itinerary, Categories
            if (existingTour == null)
            {
                return (false, "Không tìm thấy tour để cập nhật.");
            }

            // Serialize OldValues BEFORE making changes
            string oldValuesJson = SerializeToJson(new
            { // Chỉ log những trường quan trọng nếu cần
                existingTour.Name,
                existingTour.Destination,
                existingTour.AdultPrice,
                existingTour.ChildPrice,
                existingTour.Status,
                existingTour.IsCancellable,
                ImageUrls = existingTour.ImageUrls, // Lấy PublicIds cũ
                // Có thể thêm CategoryIds và Itinerary cũ vào đây nếu muốn log chi tiết hơn
            });


            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Xử lý Ảnh - Xóa ảnh cũ được yêu cầu
                var currentPublicIds = existingTour.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                if (imagesToDeletePublicIds != null && imagesToDeletePublicIds.Any())
                {
                    _logger.LogInformation("Deleting {Count} images for TourID: {TourId}", imagesToDeletePublicIds.Count, model.TourID);
                    foreach (var publicIdToDelete in imagesToDeletePublicIds)
                    {
                        if (currentPublicIds.Contains(publicIdToDelete))
                        {
                            var deleteSuccess = await _photoService.DeletePhotoAsync(publicIdToDelete);
                            if (deleteSuccess)
                            {
                                currentPublicIds.Remove(publicIdToDelete); // Xóa khỏi danh sách hiện tại
                                _logger.LogInformation("Deleted image {PublicId} for Tour {TourId}", publicIdToDelete, model.TourID);
                            }
                            else
                            {
                                _logger.LogWarning("Failed to delete image {PublicId} for Tour {TourId}", publicIdToDelete, model.TourID);
                                // Có thể dừng lại hoặc tiếp tục tùy logic
                            }
                        }
                    }
                }

                // 2. Xử lý Ảnh - Upload ảnh mới
                if (newImageFiles != null && newImageFiles.Any())
                {
                    _logger.LogInformation("Processing {Count} new uploaded images for TourID: {TourId}", newImageFiles.Count, model.TourID);
                    int imageCounter = currentPublicIds.Count + 1; // Để tên file không trùng
                    foreach (var file in newImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            string desiredPublicId = $"tours/{model.TourID}/image_{imageCounter++}_{DateTime.UtcNow.Ticks}";
                            var uploadResult = await _photoService.UploadPhotoAsync(file, $"tours/{model.TourID}", desiredPublicId);
                            if (uploadResult.Success && !string.IsNullOrEmpty(uploadResult.PublicId))
                            {
                                currentPublicIds.Add(uploadResult.PublicId); // Thêm PublicId mới vào danh sách
                            }
                            else
                            {
                                _logger.LogWarning("Failed to upload new image {FileName} for TourID {TourId}. Error: {Error}", file.FileName, model.TourID, uploadResult.ErrorMessage);
                                // Xem xét Rollback hay chỉ bỏ qua ảnh lỗi
                            }
                        }
                    }
                }

                // 3. Cập nhật Tour Entity với dữ liệu mới từ ViewModel
                existingTour.Name = model.Name;
                existingTour.Destination = model.Destination;
                existingTour.Description = model.Description;
                existingTour.DurationDays = model.DurationDays;
                existingTour.DurationNights = model.DurationNights;
                existingTour.AdultPrice = model.AdultPrice;
                existingTour.ChildPrice = model.ChildPrice;
                existingTour.MinGroupSize = model.MinGroupSize;
                existingTour.MaxGroupSize = model.MaxGroupSize;
                existingTour.DeparturePoints = model.DeparturePoints;
                existingTour.IncludedServices = model.IncludedServices;
                existingTour.ExcludedServices = model.ExcludedServices;
                existingTour.Status = model.Status;
                existingTour.IsCancellable = model.IsCancellable;
                existingTour.CancellationPolicyDescription = model.CancellationPolicyDescription;
                existingTour.ImageUrls = string.Join(";", currentPublicIds); // Cập nhật chuỗi PublicId mới

                // 4. Gọi Repo Update Tour cơ bản
                await _tourRepository.UpdateTourAsync(existingTour);
                _logger.LogInformation("Updated base tour info for TourID: {TourId}", model.TourID);


                // 5. Cập nhật Categories
                if (model.SelectedCategoryIds != null)
                {
                    await _tourRepository.UpdateTourCategoriesAsync(model.TourID, model.SelectedCategoryIds);
                    _logger.LogInformation("Updated categories for Tour {TourId}.", model.TourID);
                }

                // 6. Cập nhật Itinerary
                if (model.ItineraryDays != null) // Luôn gửi về dù rỗng nếu muốn xóa hết
                {
                    var itineraryEntities = model.ItineraryDays.Select(ivm => new ItineraryDay
                    {
                        ItineraryDayID = ivm.ItineraryDayID, // Giữ ID cũ nếu là sửa
                        TourID = model.TourID,
                        DayNumber = ivm.DayNumber,
                        Title = ivm.Title,
                        Description = ivm.Description,
                        Order = ivm.Order
                    }).ToList();
                    await _tourRepository.UpdateItineraryAsync(model.TourID, itineraryEntities); // Repo sẽ xóa cũ thêm mới/sửa
                    _logger.LogInformation("Updated itinerary for Tour {TourId}.", model.TourID);
                }

                // 7. Ghi Audit Log
                string newValuesJson = SerializeToJson(new
                { // Chỉ log những trường quan trọng
                    existingTour.Name,
                    existingTour.Destination,
                    existingTour.AdultPrice,
                    existingTour.ChildPrice,
                    existingTour.Status,
                    existingTour.IsCancellable,
                    ImageUrls = existingTour.ImageUrls // Lấy PublicIds mới
                });
                var auditLog = new TourAuditLog
                {
                    TourID = model.TourID,
                    ActionType = "UPDATE",
                    PerformedByUserID = currentUserId,
                    Timestamp = DateTime.UtcNow,
                    OldValues = oldValuesJson,
                    NewValues = newValuesJson
                };
                _context.TourAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync(); // Lưu Audit Log
                _logger.LogInformation("Audit log created for Tour {TourId} UPDATE action.", model.TourID);


                // 8. Commit Transaction
                await transaction.CommitAsync();
                _logger.LogInformation("Tour {TourId} updated successfully.", model.TourID);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating tour with ID {TourId}. Transaction rolled back.", model.TourID);
                // Cần logic phức tạp để rollback cả việc xóa/upload ảnh trên Cloudinary nếu muốn hoàn hảo
                return (false, "Đã có lỗi nghiêm trọng xảy ra khi cập nhật tour.");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteTourAsync(int tourId)
        {
            _logger.LogInformation("Attempting to delete tour with ID: {TourId}", tourId);
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0) return (false, "Không thể xác định người dùng thực hiện.");

            // 1. Kiểm tra xem Tour có đang được sử dụng không
            if (await _tourRepository.IsTourInUseAsync(tourId))
            {
                _logger.LogWarning("Cannot delete Tour {TourId} because it has active bookings or groups.", tourId);
                return (false, "Không thể xóa Tour này vì đang có Đơn đặt hoặc Đoàn đi liên quan.");
            }

            // 2. Lấy thông tin Tour để xóa ảnh và ghi log
            var tourToDelete = await _tourRepository.GetByIdAsync(tourId); // Chỉ cần lấy Tour cơ bản
            if (tourToDelete == null)
            {
                _logger.LogWarning("Tour {TourId} not found for deletion.", tourId);
                return (false, "Không tìm thấy Tour để xóa.");
            }
            string oldValuesJson = SerializeToJson(tourToDelete); // Log trạng thái trước khi xóa
            var publicIdsToDelete = tourToDelete.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 3. Ghi Audit Log trước khi xóa (để đảm bảo log được lưu nếu xóa DB lỗi)
                var auditLog = new TourAuditLog
                {
                    TourID = tourId,
                    ActionType = "DELETE",
                    PerformedByUserID = currentUserId,
                    Timestamp = DateTime.UtcNow,
                    OldValues = oldValuesJson,
                    NewValues = null // Không có giá trị mới khi xóa
                };
                _context.TourAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync(); // Lưu Audit Log ngay

                // 4. Gọi Repo xóa Tour (Repo cần xử lý xóa các entity liên quan như Itinerary, TourCategory)
                await _tourRepository.DeleteTourAsync(tourId);
                _logger.LogInformation("Deleted Tour {TourId} from database.", tourId);

                // 5. Commit transaction CSDL
                await transaction.CommitAsync();

                // 6. Xóa ảnh trên Cloudinary (thực hiện sau khi commit DB thành công)
                if (publicIdsToDelete.Any())
                {
                    _logger.LogInformation("Deleting {Count} images from Cloudinary for deleted TourID: {TourId}", publicIdsToDelete.Count, tourId);
                    foreach (var publicId in publicIdsToDelete)
                    {
                        await _photoService.DeletePhotoAsync(publicId); // Log lỗi nếu có nhưng không cần rollback DB
                    }
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback thay đổi CSDL
                _logger.LogError(ex, "Error deleting tour with ID {TourId}. Transaction rolled back.", tourId);
                return (false, "Đã có lỗi xảy ra khi xóa tour.");
            }
        }
        public async Task<DeleteTourConfirmationViewModel?> GetTourForDeleteConfirmationAsync(int tourId)
        {
            _logger.LogInformation("Fetching tour basic info for delete confirmation. TourID: {TourId}", tourId);
            var tour = await _tourRepository.GetByIdAsync(tourId); // Chỉ cần lấy thông tin cơ bản
            if (tour == null)
            {
                return null;
            }

            // Kiểm tra xem tour có đang được dùng không
            bool isInUse = await _tourRepository.IsTourInUseAsync(tourId);

            return new DeleteTourConfirmationViewModel
            {
                TourId = tour.TourID,
                TourName = tour.Name,
                IsInUse = isInUse
            };
        }
    }
}
