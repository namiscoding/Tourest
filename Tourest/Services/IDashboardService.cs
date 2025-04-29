using Tourest.ViewModels.Admin.AdminDashboard;

namespace Tourest.Services
{
    public enum TimePeriod { Last7Days, Last30Days, CurrentMonth, CurrentYear } 

    public interface IDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardDataAsync(TimePeriod period = TimePeriod.Last30Days);
    }
}
