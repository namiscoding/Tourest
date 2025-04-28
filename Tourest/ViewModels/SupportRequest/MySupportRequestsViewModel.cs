namespace Tourest.ViewModels.SupportRequest
{
    public class MySupportRequestsViewModel
    {
        public List<SupportRequestSummaryViewModel> Requests { get; set; } = new List<SupportRequestSummaryViewModel>();
        public CreateSupportRequestViewModel? NewRequest { get; set; }

    }
}
