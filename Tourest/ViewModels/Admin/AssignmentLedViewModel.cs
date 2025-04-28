namespace Tourest.ViewModels.Admin
{
    public class AssignmentLedViewModel // Dùng trong TourGuideDetails
    {
        public int AssignmentId { get; set; }
        public string TourName { get; set; } = "N/A";
        public DateTime? DepartureDate { get; set; }
        public string AssignmentStatus { get; set; } = string.Empty;
    }
}
