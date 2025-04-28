using Tourest.ViewModels.SupportRequest;

namespace Tourest.Services
{
    public interface ISupportRequestService
    {
        Task CreateSupportRequestAsync(CreateSupportRequestViewModel model, int customerId);
        Task<MySupportRequestsViewModel> GetMyRequestsViewModelAsync(int customerId);
    }
}
