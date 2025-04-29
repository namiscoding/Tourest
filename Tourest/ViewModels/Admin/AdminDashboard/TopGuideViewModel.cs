namespace Tourest.ViewModels.Admin.AdminDashboard
{
    public class TopGuideViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal? AverageRating { get; set; }
        public int RatingCount { get; set; }    
        public int AssignmentCount { get; set; }
    }
}
