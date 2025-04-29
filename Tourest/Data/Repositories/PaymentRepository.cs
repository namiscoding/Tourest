using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(ApplicationDbContext context, ILogger<PaymentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Payment?> FindByIdAsync(int paymentId)
        {
            return await _context.Payments.FindAsync(paymentId);
        }

        public async Task<Payment?> GetPaymentDetailsByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Customer) // Include User (Customer)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Tour) // Include Tour
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PaymentID == paymentId);
        }

        public async Task<(IEnumerable<Payment> Payments, int TotalCount)> GetPaymentsPagedAsync(
            Expression<Func<Payment, bool>>? filter = null,
            Func<IQueryable<Payment>, IOrderedQueryable<Payment>>? orderBy = null,
            int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<Payment> query = _context.Payments
                                        .Include(p => p.Booking)
                                            .ThenInclude(b => b.Customer)
                                        .Include(p => p.Booking)
                                            .ThenInclude(b => b.Tour);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                query = query.OrderByDescending(p => p.PaymentDate); // Mặc định sắp xếp mới nhất
            }

            var payments = await query.Skip((pageIndex - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            return (payments, totalCount);
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            try
            {
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating Payment {PaymentId}", payment.PaymentID);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Payment {PaymentId}", payment.PaymentID);
                return false;
            }
        }
        public async Task<int> GetTotalRevenueAsync(DateTime start, DateTime end)
        {
            _logger.LogInformation("Calculating total revenue between {StartDate} and {EndDate}", start.ToShortDateString(), end.ToShortDateString());
            DateTime adjustedEndDate = end.Date.AddDays(1);
            try
            {
                // Sum cột Amount (kiểu INT) của các Payment thành công trong khoảng thời gian
                return await _context.Payments
                    .Where(p => p.Status == "Completed" && p.PaymentDate >= start.Date && p.PaymentDate < adjustedEndDate)
                    .SumAsync(p => p.Amount); // Sum kiểu INT
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total revenue.");
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetRevenueGroupedByMonthAsync(DateTime start, DateTime end)
        {
            _logger.LogInformation("Getting revenue grouped by month between {StartDate} and {EndDate}", start.ToShortDateString(), end.ToShortDateString());
            DateTime adjustedEndDate = end.Date.AddDays(1);
            try
            {
                var monthlyRevenue = await _context.Payments
                    .Where(p => p.Status == "Completed" && p.PaymentDate >= start.Date && p.PaymentDate < adjustedEndDate)
                    .GroupBy(p => new { Year = p.PaymentDate.Year, Month = p.PaymentDate.Month }) // Nhóm theo Năm và Tháng
                    .Select(g => new {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Total = g.Sum(p => p.Amount) // Tính tổng doanh thu mỗi tháng
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month) // Sắp xếp theo thời gian
                    .ToDictionaryAsync(x => $"{x.Year}-{x.Month:D2}", x => x.Total); // Key là string "YYYY-MM"

                // Điền các tháng không có doanh thu bằng 0 (quan trọng cho biểu đồ liên tục)
                var filledRevenue = new Dictionary<string, int>();
                // Lấy tháng/năm bắt đầu và kết thúc
                var currentMonth = new DateTime(start.Year, start.Month, 1);
                var lastMonth = new DateTime(end.Year, end.Month, 1);

                while (currentMonth <= lastMonth)
                {
                    var monthString = currentMonth.ToString("yyyy-MM");
                    filledRevenue[monthString] = monthlyRevenue.GetValueOrDefault(monthString, 0);
                    currentMonth = currentMonth.AddMonths(1);
                }

                return filledRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue grouped by month.");
                return new Dictionary<string, int>();
            }
        }
    }
}
