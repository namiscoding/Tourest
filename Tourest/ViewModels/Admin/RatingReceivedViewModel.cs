namespace Tourest.ViewModels.Admin
{
    public class RatingReceivedViewModel // Dùng trong TourGuideDetails
    {
        public int RatingId { get; set; }
        public decimal RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime RatingDate { get; set; }
        public string CustomerName { get; set; } = "N/A";
    }
}
