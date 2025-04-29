using Tourest.Helpers;
using Tourest.ViewModels.Admin;

namespace Tourest.Services
{
    public interface IPaymentService
    {
        Task<PaginatedList<AdminPaymentListViewModel>> GetPaymentsForAdminAsync(
            string? methodFilter, DateTime? startDate, DateTime? endDate,
            string? searchTerm, string? sortBy, bool ascending,
            int pageIndex, int pageSize);

        Task<AdminPaymentDetailsViewModel?> GetPaymentDetailsForAdminAsync(int paymentId);

    }
}
