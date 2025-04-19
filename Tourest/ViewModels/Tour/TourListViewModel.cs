namespace Tourest.ViewModels.Tour
{
    public class TourListViewModel
    {
        public int TourId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int DurationDays { get; set; }
        public int ChildPrice { get; set; } 
        public string? ThumbnailImageUrl { get; set; } 
        public decimal? AverageRating { get; set; } 
        public int? SumRating { get; set; }
    }
}
