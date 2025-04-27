using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tourest.Data; 
using Microsoft.EntityFrameworkCore;
namespace Tourest.BackgroundServices
{
    public class BookingStatusUpdaterService : BackgroundService
    {
        private readonly ILogger<BookingStatusUpdaterService> _logger;
        private readonly IServiceScopeFactory _scopeFactory; // Dùng để tạo scope cho DbContext (Scoped service)

        // Định nghĩa các hằng số cho trạng thái để tránh lỗi chính tả
        private const string StatusConfirmed = "Paid";
        private const string StatusPendingPayment = "PendingPayment";
        private const string StatusCompleted = "Completed";
        private const string StatusCancelled = "Cancelled";


        public BookingStatusUpdaterService(ILogger<BookingStatusUpdaterService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Status Updater Service is starting.");

            // Chạy định kỳ, ví dụ mỗi 24 giờ
            // Hoặc dùng PeriodicTimer để kiểm soát tốt hơn
            using var timer = new PeriodicTimer(TimeSpan.FromHours(24)); // Chạy mỗi ngày

            // Tính toán thời gian chờ đến nửa đêm lần đầu tiên (tùy chọn)
            // var now = DateTime.UtcNow;
            // var nextRunTime = now.Date.AddDays(1); // Nửa đêm ngày hôm sau (UTC)
            // var initialDelay = nextRunTime - now;
            // if (initialDelay.TotalMilliseconds > 0) {
            //     _logger.LogInformation("Booking Status Updater: Initial delay until {NextRunTime} UTC.", nextRunTime);
            //     await Task.Delay(initialDelay, stoppingToken);
            // }


            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation("Booking Status Updater Service is running at: {time}", DateTimeOffset.Now);

                    // Vì BackgroundService là Singleton, còn DbContext là Scoped,
                    // chúng ta cần tạo một scope mới để lấy DbContext trong mỗi lần chạy.
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var today = DateTime.Today; // Lấy ngày hiện tại (chỉ phần Date)

                        // Tìm các booking cần cập nhật:
                        // - DepartureDate đã qua (nhỏ hơn ngày hôm nay)
                        // - Trạng thái là Confirmed hoặc PendingPayment
                        //var bookingsToUpdate = await dbContext.Bookings
                        //    .Where(b => b.DepartureDate < today &&
                        //                (b.Status == StatusConfirmed || b.Status == StatusPendingPayment))
                        //    .ToListAsync(stoppingToken); // Truyền stoppingToken

                        var bookingsToUpdate = await dbContext.Bookings
                            .Where(b => b.DepartureDate >= today && // <<< Đổi thành >= để test ngày chưa qua
                            (b.Status == StatusConfirmed || b.Status == StatusPendingPayment))
                            .ToListAsync(stoppingToken);

                        if (bookingsToUpdate.Any())
                        {
                            _logger.LogInformation("Found {Count} bookings to update status.", bookingsToUpdate.Count);
                            foreach (var booking in bookingsToUpdate)
                            {
                                if (booking.Status == StatusConfirmed)
                                {
                                    booking.Status = StatusCompleted; // Đổi sang Hoàn thành
                                    _logger.LogInformation("Booking ID {BookingID} status updated to Completed.", booking.BookingID);
                                }
                                else if (booking.Status == StatusPendingPayment)
                                {
                                    booking.Status = StatusCancelled; // Đổi sang Hủy
                                    booking.CancellationDate = DateTime.UtcNow; // Ghi lại ngày hủy (giờ UTC)
                                                                                // Có thể thêm logic hoàn tiền hoặc không tùy quy định
                                    booking.RefundAmount = 0; // Ví dụ: Không hoàn tiền cho pending quá hạn
                                    _logger.LogInformation("Booking ID {BookingID} status updated to Cancelled.", booking.BookingID);
                                }
                                // EF Core tự động theo dõi thay đổi vì đã lấy booking có tracking
                            }

                            // Lưu tất cả thay đổi vào database
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Successfully saved status updates for {Count} bookings.", bookingsToUpdate.Count);
                        }
                        else
                        {
                            _logger.LogInformation("No bookings found requiring status update.");
                        }
                    } // Scope được giải phóng ở đây
                }
                catch (OperationCanceledException)
                {
                    // Khi stoppingToken được kích hoạt (ứng dụng dừng), thoát vòng lặp một cách nhẹ nhàng
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating booking statuses.");
                    // Đợi một khoảng thời gian ngắn trước khi thử lại để tránh lỗi liên tục nếu có vấn đề DB
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Chờ 5 phút trước khi chạy lại
                }

                // Đợi cho lần chạy tiếp theo (được quản lý bởi PeriodicTimer)
                // Hoặc nếu dùng Task.Delay:
                // await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Chờ 24 tiếng
            }

            _logger.LogInformation("Booking Status Updater Service is stopping.");
        }
    }
}
