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
        private const string StatusConfirmed = "Paid";
        private const string StatusPendingPayment = "PendingPayment";
        private const string StatusCompleted = "Completed";
        private const string StatusCancelled = "Cancelled";


        public BookingProcessingService(IServiceScopeFactory scopeFactory, ILogger<BookingProcessingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // Hàm xử lý logic cập nhật chính
        public async Task<(int updatedCount, string message)> UpdateStatusesForPastDeparturesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting manual booking status update process...");
            int updatedCount = 0;
            try
            {
                // Tạo scope để lấy DbContext
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var today = DateTime.Today;

                    var bookingsToUpdate = await dbContext.Bookings
                        .Where(b => b.DepartureDate < today &&
                                    (b.Status == StatusConfirmed || b.Status == StatusPendingPayment))
                         .ToListAsync(cancellationToken); // Pass CancellationToken

                    if (bookingsToUpdate.Any())
                    {
                        _logger.LogInformation("Found {Count} bookings to update status manually.", bookingsToUpdate.Count);
                        foreach (var booking in bookingsToUpdate)
                        {
                            if (booking.Status == StatusConfirmed)
                            {
                                booking.Status = StatusCompleted;
                            }
                            else if (booking.Status == StatusPendingPayment)
                            {
                                booking.Status = StatusCancelled;
                                booking.CancellationDate = DateTime.UtcNow;
                                booking.RefundAmount = 0; // Hoặc logic khác
                            }
                        }
                        updatedCount = await dbContext.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("Successfully saved status updates for {Count} bookings manually.", updatedCount);
                        return (updatedCount, $"Đã cập nhật trạng thái cho {updatedCount} booking.");
                    }
                    else
                    {
                        _logger.LogInformation("No bookings found requiring status update manually.");
                        return (0, "Không có booking nào cần cập nhật trạng thái.");
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
                return (0, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}