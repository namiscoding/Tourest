
using Tourest.Helpers;
using Tourest.ViewModels.Admin;

namespace Tourest.Services
{
    public interface IUserService
    {
        // Customer Management
        Task<PaginatedList<AdminCustomerViewModel>> GetCustomersForAdminAsync(int pageIndex = 1, int pageSize = 10, string searchTerm = "");
        Task<AdminCustomerDetailsViewModel?> GetCustomerDetailsForAdminAsync(int userId);
        Task<(bool Success, string ErrorMessage)> CreateCustomerByAdminAsync(AdminCreateCustomerViewModel model);
        Task<EditCustomerViewModel?> GetCustomerForEditAsync(int userId);
        Task<(bool Success, string ErrorMessage)> UpdateCustomerByAdminAsync(EditCustomerViewModel model);

        // Tour Manager Management
        Task<PaginatedList<AdminTourManagerViewModel>> GetTourManagersForAdminAsync(int pageIndex = 1, int pageSize = 10, string searchTerm = "");
        Task<AdminTourManagerDetailsViewModel?> GetTourManagerDetailsForAdminAsync(int userId); // Method mới cho Details
        Task<(bool Success, string ErrorMessage)> CreateTourManagerByAdminAsync(AdminCreateTourManagerViewModel model); // Method mới cho Create
        Task<EditTourManagerViewModel?> GetTourManagerForEditAsync(int userId); // Method mới cho Edit GET
        Task<(bool Success, string ErrorMessage)> UpdateTourManagerByAdminAsync(EditTourManagerViewModel model); // Method mới cho Edit POST

        // General User Actions
        Task<bool> ToggleUserActiveStatusAsync(int userId);
    }
}
