using System.Linq.Expressions;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface IPaymentRepository
    {
        // Lấy danh sách Payments kèm thông tin liên quan, có phân trang, lọc, sắp xếp
        Task<(IEnumerable<Payment> Payments, int TotalCount)> GetPaymentsPagedAsync(
            Expression<Func<Payment, bool>>? filter = null, // Bộ lọc LINQ
            Func<IQueryable<Payment>, IOrderedQueryable<Payment>>? orderBy = null, // Sắp xếp LINQ
            int pageIndex = 1,
            int pageSize = 10);

        // Lấy chi tiết Payment bằng ID, kèm thông tin Booking, Customer, Tour
        Task<Payment?> GetPaymentDetailsByIdAsync(int paymentId);

        // Tìm Payment bằng ID (cho việc cập nhật)
        Task<Payment?> FindByIdAsync(int paymentId);

        // Cập nhật Payment (chủ yếu là Status)
        Task<bool> UpdateAsync(Payment payment);
        Task<int> GetTotalRevenueAsync(DateTime start, DateTime end);
        Task<Dictionary<string, int>> GetRevenueGroupedByMonthAsync(DateTime start, DateTime end); // Trả về Dictionary<MonthYearString, Sum>

    }
}