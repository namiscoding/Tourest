namespace Tourest.ViewModels.Tour
{
    public class TourListAllViewModel
    {
        public int TourID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationDays { get; set; }
        public int DurationNights { get; set; }

        // [Column(TypeName = "decimal(18, 2)")]
        public int AdultPrice { get; set; }

        // [Column(TypeName = "decimal(18, 2)")]
        public int ChildPrice { get; set; }

        public int? MinGroupSize { get; set; }
        public int? MaxGroupSize { get; set; }
        public string? DeparturePoints { get; set; }
        public string? IncludedServices { get; set; }
        public string? ExcludedServices { get; set; }
        public string? ImageUrls { get; set; }
        public string Status { get; set; } = string.Empty;

        // [Column(TypeName = "decimal(3, 2)")]
        public decimal? AverageRating { get; set; }

        public bool IsCancellable { get; set; }
        public string? CancellationPolicyDescription { get; set; }
    }
}
