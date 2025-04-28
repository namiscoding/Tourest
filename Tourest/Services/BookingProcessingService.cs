// File: Services/BookingProcessingService.cs
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tourest.Data;
using Microsoft.EntityFrameworkCore;

namespace Tourest.Services
{
    public class BookingProcessingService : IBookingProcessingService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingProcessingService> _logger;

        // Định nghĩa các hằng số trạng thái
        private const string StatusPaid = "Paid"; // <<< THAY ĐỔI: Dùng "Paid" thay vì "Confirmed"
        private const string StatusPendingPayment = "PendingPayment";
        private const string StatusCompleted = "Completed";
        private const string StatusCancelled = "Cancelled";
        // private const string StatusPendingAssignment = "PendingAssignment"; // Không cần xử lý trạng thái này

        public BookingProcessingService(IServiceScopeFactory scopeFactory, ILogger<BookingProcessingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<(int updatedCount, string message)> UpdateStatusesForPastDeparturesAsync(CancellationToken cancellationToken = default)
        {
            // Thêm ghi chú log cho chế độ test
            _logger.LogInformation("Starting booking status update process (TEST MODE - Processing future/today departures)...");
            int updatedCount = 0;
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var today = DateTime.Today;

                    // === THAY ĐỔI ĐIỀU KIỆN LỌC ĐỂ TEST ===
                    var bookingsToUpdate = await dbContext.Bookings
                        .Where(b => b.DepartureDate >= today && // <<< ĐẢO NGƯỢC ĐIỀU KIỆN NGÀY ĐỂ TEST
                                    (b.Status == StatusPaid || b.Status == StatusPendingPayment)) // <<< Chỉ kiểm tra Paid hoặc PendingPayment
                        .ToListAsync(cancellationToken);
                    // === KẾT THÚC THAY ĐỔI ===

                    if (bookingsToUpdate.Any())
                    {
                        _logger.LogInformation("[TEST MODE] Found {Count} future/today bookings with status 'Paid' or 'PendingPayment' to update.", bookingsToUpdate.Count);
                        foreach (var booking in bookingsToUpdate)
                        {
                            // === CẬP NHẬT LOGIC ĐỔI TRẠNG THÁI ===
                            if (booking.Status == StatusPaid)
                            {
                                booking.Status = StatusCompleted; // Đổi Paid -> Completed
                                _logger.LogInformation("[TEST MODE] Booking ID {BookingID} status updated to Completed (Original: Paid).", booking.BookingID);
                            }
                            else if (booking.Status == StatusPendingPayment)
                            {
                                booking.Status = StatusCancelled; // Đổi PendingPayment -> Cancelled
                                booking.CancellationDate = DateTime.UtcNow;
                                booking.RefundAmount = 0; // Hoặc logic khác
                                _logger.LogInformation("[TEST MODE] Booking ID {BookingID} status updated to Cancelled (Original: PendingPayment).", booking.BookingID);
                            }
                            // === KẾT THÚC CẬP NHẬT LOGIC ===
                        }

                        updatedCount = await dbContext.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("[TEST MODE] Successfully saved status updates for {Count} bookings.", updatedCount);
                        return (updatedCount, $"[TEST MODE] Đã cập nhật trạng thái cho {updatedCount} booking (ngày chưa qua).");
                    }
                    else
                    {
                        _logger.LogInformation("[TEST MODE] No future/today bookings with status 'Paid' or 'PendingPayment' found requiring update.");
                        return (0, "[TEST MODE] Không có booking (ngày chưa qua, trạng thái Paid/PendingPayment) nào cần cập nhật.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Manual booking status update was cancelled.");
                return (0, "Quá trình cập nhật đã bị hủy.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual booking status update.");
                return (-1, $"Lỗi hệ thống: {ex.Message}"); // Trả về -1 để báo lỗi
            }
        }
    }
}