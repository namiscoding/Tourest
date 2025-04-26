using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.ViewModels;
using Tourest.ViewModels.Booking;

namespace Tourest.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITourGroupRepository _tourGroupRepository;
        private readonly ApplicationDbContext _context;
        // Inject các service/repo khác nếu cần

        public BookingService(
            IBookingRepository bookingRepository,
            ITourRepository tourRepository,
            ITourGroupRepository tourGroupRepository,
            ApplicationDbContext context) // Inject DbContext
        {
            _bookingRepository = bookingRepository;
            _tourRepository = tourRepository;
            _tourGroupRepository = tourGroupRepository;
            _context = context; // Lưu lại DbContext
        }

        public async Task<BookingHistoryViewModel> GetBookingHistoryAsync(int customerId, string? destinationFilter = null)
        {
            // 1. Lấy TẤT CẢ booking của user để lấy danh sách destination duy nhất
            var allUserBookings = await _bookingRepository.GetAllBookingsByCustomerIdAsync(customerId);

            // 2. Tạo danh sách Destination duy nhất để lọc
            var distinctDestinations = allUserBookings
                .Where(b => b.Tour != null && !string.IsNullOrEmpty(b.Tour.Destination)) // Bỏ qua tour null hoặc destination rỗng
                .Select(b => b.Tour.Destination)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            // 3. Lọc danh sách booking nếu có destinationFilter
            IEnumerable<Data.Entities.Booking> filteredBookings = allUserBookings; // Bắt đầu với danh sách đầy đủ
            if (!string.IsNullOrEmpty(destinationFilter))
            {
                filteredBookings = allUserBookings
                                  .Where(b => b.Tour != null && b.Tour.Destination.Equals(destinationFilter, StringComparison.OrdinalIgnoreCase)); // Lọc không phân biệt hoa thường
            }


            // === TÍNH TOÁN DỮ LIỆU BIỂU ĐỒ ===
            var completedStatuses = new[] { "Paid", "Completed" }; // Các trạng thái tính chi tiêu

            // 4. Tính tổng chi tiêu (Confirmed/Completed)
            var totalSpent = filteredBookings // Tính trên danh sách đã lọc hoặc allUserBookings tùy yêu cầu
                .Where(b => completedStatuses.Contains(b.Status, StringComparer.OrdinalIgnoreCase))
                .Sum(b => (long)b.TotalPrice); // Sử dụng long để tránh tràn số nếu tổng tiền lớn

            // 5. Tính chi tiêu hàng tháng (Confirmed/Completed)
            
            var targetYear = DateTime.Now.Year; 

            
            var yearlyBookings = filteredBookings // Lọc trên danh sách đã lọc theo destination, hoặc dùng allUserBookings nếu muốn tính tổng cả năm không filter
                .Where(b => completedStatuses.Contains(b.Status, StringComparer.OrdinalIgnoreCase) && b.BookingDate.Year == targetYear);

            // Nhóm theo tháng và tính tổng chi tiêu cho mỗi tháng có dữ liệu
            var monthlySums = yearlyBookings
                .GroupBy(b => b.BookingDate.Month) // Chỉ cần nhóm theo tháng
                .ToDictionary(g => g.Key, g => g.Sum(b => b.TotalPrice)); // Tạo Dictionary<tháng, tổng tiền>

            // Tạo danh sách kết quả cho đủ 12 tháng
            var monthlySpendingFullYear = new List<MonthlySpendingViewModel>();
            for (int month = 1; month <= 12; month++)
            {
                monthlySpendingFullYear.Add(new MonthlySpendingViewModel
                {
                    Year = targetYear,
                    Month = month,
                    MonthYearLabel = $"T{month}/{targetYear}", // Nhãn dạng "TX/YYYY"
                    TotalAmount = monthlySums.ContainsKey(month) ? monthlySums[month] : 0 // Lấy tổng tiền hoặc gán 0 nếu không có
                });
            }
            
            // 4. Map danh sách đã lọc sang Item ViewModel
            var bookingItems = filteredBookings
                .Select(b => new BookingHistoryItemViewModel
                {
                    BookingId = b.BookingID,
                    BookingDate = b.BookingDate,
                    DepartureDate = b.DepartureDate,
                    TourId = b.TourID,
                    TourName = b.Tour?.Name ?? "N/A", 
                    Destination = b.Tour?.Destination ?? "N/A", 
                    PickupPoint = b.PickupPoint,
                    TotalPrice = b.TotalPrice,
                    NumberOfAdults = b.NumberOfAdults,
                    NumberOfChildren = b.NumberOfChildren,
                    Status = b.Status,
                    CancellationDate = b.CancellationDate,
                    RefundAmount = b.RefundAmount
                }).ToList();

            // 5. Tạo ViewModel chính
            var viewModel = new BookingHistoryViewModel
            {
                Bookings = bookingItems,
                AvailableDestinations = distinctDestinations,
                SelectedDestinationFilter = destinationFilter,
                TotalSpentConfirmedCompleted = (int)totalSpent, 
                MonthlySpendingData = monthlySpendingFullYear
            };

            return viewModel;
        }

        // --- IMPLEMENT PHƯƠNG THỨC MỚI ---
        public async Task<(bool Success, string ErrorMessage, int? BookingId)> CreateBookingAsync(CreateBookingViewModel model, int customerId)
        {
            // Dùng transaction để đảm bảo hoặc tất cả thành công, hoặc không có gì thay đổi
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Lấy thông tin Tour, kiểm tra tồn tại, trạng thái và MaxGroupSize
                // Cần đảm bảo GetByIdAsync tải cả MaxGroupSize
                var tour = await _tourRepository.GetByIdAsync(model.TourId);
                if (tour == null || tour.Status != "Active")
                {
                    await transaction.RollbackAsync();
                    return (false, "Tour không hợp lệ hoặc không hoạt động.", null);
                }

                // 2. Chuẩn hóa ngày khởi hành (chỉ lấy Date) và kiểm tra
                var departureDate = model.SelectedDate.Date;
                if (departureDate < DateTime.Today)
                {
                    await transaction.RollbackAsync();
                    return (false, "Ngày khởi hành không thể là ngày trong quá khứ.", null);
                }

                int totalNewGuests = model.NumberOfAdults + model.NumberOfChildren;
                if (totalNewGuests <= 0)
                {
                    await transaction.RollbackAsync();
                    return (false, "Số lượng khách phải lớn hơn 0.", null);
                }


                // 3. Tìm hoặc Tạo TourGroup
                TourGroup? tourGroup = await _tourGroupRepository.FindByTourAndDateAsync(model.TourId, departureDate);
                bool isNewGroup = false;

                if (tourGroup == null) // Nếu chưa có nhóm cho ngày này -> Tạo nhóm mới
                {
                    // Kiểm tra sức chứa nếu tạo nhóm mới
                    if (tour.MaxGroupSize.HasValue && totalNewGuests > tour.MaxGroupSize.Value)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Số lượng khách ({totalNewGuests}) vượt quá giới hạn tối đa ({tour.MaxGroupSize.Value}) của tour.", null);
                    }

                    tourGroup = new TourGroup
                    {
                        TourID = model.TourId,
                        DepartureDate = departureDate,
                        TotalGuests = 0, // Sẽ cộng dồn sau
                        Status = "Scheduled", // Trạng thái ban đầu của nhóm
                        CreationDate = DateTime.UtcNow
                    };
                    await _tourGroupRepository.AddAsync(tourGroup);
                    await _context.SaveChangesAsync(); // Lưu để lấy TourGroupID mới được tạo
                    isNewGroup = true;
                }
                else // Nhóm đã tồn tại
                {
                    // Kiểm tra sức chứa của nhóm hiện tại trước khi thêm khách mới
                    // Tính toán lại tổng số khách hiện tại trong nhóm cho chắc chắn (trừ các booking đã hủy)
                    int confirmedGuestsInGroup = await _context.Bookings
                                          .Where(b => b.TourGroupID == tourGroup.TourGroupID && b.Status == "Paid") // Chỉ đếm "Confirmed"
                                          .SumAsync(b => b.NumberOfAdults + b.NumberOfChildren);

                    int potentialTotalGuests = confirmedGuestsInGroup + totalNewGuests; // Tổng tiềm năng nếu thêm khách mới

                    if (tour.MaxGroupSize.HasValue && potentialTotalGuests > tour.MaxGroupSize.Value)
                    {
                        await transaction.RollbackAsync();
                        // Thông báo lỗi rõ hơn: số chỗ đã xác nhận và số chỗ còn lại
                        int availableSlots = tour.MaxGroupSize.Value - confirmedGuestsInGroup;
                        return (false, $"Không đủ chỗ trống. Chỉ còn {availableSlots} chỗ trống đã xác nhận cho ngày này. Yêu cầu của bạn là {totalNewGuests} khách.", null);
                    }
                }

                // 4. Tính lại TotalPrice ở backend dựa trên giá từ DB
                int calculatedTotalPrice = (model.NumberOfAdults * tour.AdultPrice) + (model.NumberOfChildren * tour.ChildPrice);

                // 5. Tạo đối tượng Booking mới
                var newBooking = new Booking
                {
                    BookingDate = DateTime.UtcNow,
                    DepartureDate = departureDate,
                    NumberOfAdults = model.NumberOfAdults,
                    NumberOfChildren = model.NumberOfChildren,
                    TotalPrice = calculatedTotalPrice, // Dùng giá đã tính lại
                    Status = "PendingPayment", // Luôn bắt đầu bằng trạng thái này
                    PickupPoint = model.SelectedPickupPoint,
                    CustomerID = customerId, // Gán cứng ID=4 hoặc ID lấy từ user
                    TourID = model.TourId,
                    TourGroupID = null // Gán ID của nhóm
                };

                // 6. Thêm Booking
                await _bookingRepository.AddAsync(newBooking);

                // 7. Cập nhật lại TotalGuests cho TourGroup (cộng dồn khách mới)
                //tourGroup.TotalGuests += totalNewGuests;
                if (!isNewGroup) // Chỉ cần đánh dấu Update nếu group không phải mới tạo
                {
                    await _tourGroupRepository.UpdateAsync(tourGroup); 
                }

                // 8. Lưu tất cả thay đổi vào DB
                await _context.SaveChangesAsync();

                // 9. Commit transaction
                await transaction.CommitAsync();

                // 10. Trả về thành công
                return (true, "Yêu cầu đặt tour thành công. Vui lòng tiến hành thanh toán.", newBooking.BookingID);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log lỗi ex chi tiết
                Console.WriteLine($"Error creating booking: {ex.Message}"); // Log tạm ra console
                return (false, "Đã có lỗi hệ thống xảy ra trong quá trình đặt tour.", null);
            }
        }
        // --- KẾT THÚC IMPLEMENT ---
    }
}
