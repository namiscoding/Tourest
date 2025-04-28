namespace Tourest.ViewModels.TourManager
{
    public class TourGuideAssignmentViewModel
    {
        public DateTime AssignmentDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string? Status { get; set; }
        public string? RejectionReason { get; set; }
    }

}
