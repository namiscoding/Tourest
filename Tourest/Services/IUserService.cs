
using Tourest.Helpers;
using Tourest.ViewModels.Admin;

namespace Tourest.Services
{
    public interface IUserService
    {
        Task<PaginatedList<AdminCustomerViewModel>> GetCustomersForAdminAsync(int pageIndex = 1, int pageSize = 10, string searchTerm = "");
        Task<AdminCustomerDetailsViewModel?> GetCustomerDetailsForAdminAsync(int userId);
        Task<(bool Success, string ErrorMessage)> CreateCustomerByAdminAsync(AdminCreateCustomerViewModel model);
        Task<EditCustomerViewModel?> GetCustomerForEditAsync(int userId);
        Task<(bool Success, string ErrorMessage)> UpdateCustomerByAdminAsync(EditCustomerViewModel model);
        Task<bool> ToggleUserActiveStatusAsync(int userId);

    }
}
