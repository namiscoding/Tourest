using System.Linq.Expressions;
using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Data;
using Tourest.Helpers;
using Tourest.ViewModels.Admin;
using Tourest.Util;
namespace Tourest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ApplicationDbContext _context; // Inject DbContext để xử lý Transaction và update Booking
        private readonly ILogger<PaymentService> _logger;
        // private readonly IMapper _mapper; // Inject AutoMapper nếu dùng

        public PaymentService(IPaymentRepository paymentRepository, ApplicationDbContext context, ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<AdminPaymentDetailsViewModel?> GetPaymentDetailsForAdminAsync(int paymentId)
        {
            _logger.LogInformation("Fetching payment details for admin. PaymentID: {PaymentId}", paymentId);
            var payment = await _paymentRepository.GetPaymentDetailsByIdAsync(paymentId);
            if (payment == null) return null;

            // Manual Mapping (hoặc dùng AutoMapper)
            return new AdminPaymentDetailsViewModel
            {
                PaymentId = payment.PaymentID,
                BookingId = payment.BookingID,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionID,
                CustomerId = payment.Booking?.CustomerID,
                CustomerName = payment.Booking?.Customer?.FullName ?? "N/A",
                CustomerEmail = payment.Booking?.Customer?.Email,
                TourId = payment.Booking?.TourID,
                TourName = payment.Booking?.Tour?.Name ?? "N/A",
                BookingDepartureDate = payment.Booking?.DepartureDate,
                BookingNumberOfGuests = (payment.Booking?.NumberOfAdults ?? 0) + (payment.Booking?.NumberOfChildren ?? 0)
            };
        }

        public async Task<PaginatedList<AdminPaymentListViewModel>> GetPaymentsForAdminAsync(
           string? methodFilter, DateTime? startDate, DateTime? endDate,
           string? searchTerm, string? sortBy, bool ascending,
           int pageIndex, int pageSize)
        {
            _logger.LogInformation("Fetching COMPLETED payments for admin list..."); // Nhấn mạnh là chỉ lấy Completed

            // 1. Build Filter Expression (Bỏ phần lọc theo Status)
            Expression<Func<Payment, bool>>? filter = p => p.Status == "Completed"; // Luôn lấy Completed

            // ... (logic lọc theo methodFilter, startDate, endDate, searchTerm giữ nguyên) ...
            if (!string.IsNullOrEmpty(methodFilter)) { filter = filter.And(p => p.PaymentMethod == methodFilter); }
            if (startDate.HasValue) { filter = filter.And(p => p.PaymentDate.Date >= startDate.Value.Date); }
            if (endDate.HasValue) { filter = filter.And(p => p.PaymentDate.Date <= endDate.Value.Date); }
            if (!string.IsNullOrEmpty(searchTerm)) {/*... logic search ...*/}


            // 2. Build OrderBy Expression (Bỏ phần sort theo Status)
            Func<IQueryable<Payment>, IOrderedQueryable<Payment>>? orderBy = null;
            if (!string.IsNullOrEmpty(sortBy) && sortBy.ToLower() != "status") // Bỏ qua sort by status
            {
                switch (sortBy.ToLower())
                {
                    case "date": orderBy = q => ascending ? q.OrderBy(p => p.PaymentDate) : q.OrderByDescending(p => p.PaymentDate); break;
                    case "amount": orderBy = q => ascending ? q.OrderBy(p => p.Amount) : q.OrderByDescending(p => p.Amount); break;
                    // case "status": Bỏ qua
                    default: orderBy = q => q.OrderByDescending(p => p.PaymentDate); break;
                }
            }
            else { orderBy = q => q.OrderByDescending(p => p.PaymentDate); }


            // 3. Call Repository
            var (payments, totalCount) = await _paymentRepository.GetPaymentsPagedAsync(filter, orderBy, pageIndex, pageSize);

            // 4. Map to ViewModel (Không cần map Status)
            var viewModels = payments.Select(p => new AdminPaymentListViewModel
            {
                PaymentId = p.PaymentID,
                BookingId = p.BookingID,
                CustomerName = p.Booking?.Customer?.FullName ?? "N/A",
                TourName = p.Booking?.Tour?.Name ?? "N/A",
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionID,
                PaymentDate = p.PaymentDate
                // Status = p.Status // Bỏ dòng này
            }).ToList();

            return new PaginatedList<AdminPaymentListViewModel>(viewModels, totalCount, pageIndex, pageSize);
        }

    }
}
