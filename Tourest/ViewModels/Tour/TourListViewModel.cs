using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Tour
{
    public class TourListViewModel
    {
        public int TourId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Destination { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day.")]
        public int DurationDays { get; set; }

        [Range(0, int.MaxValue)]
        public int DurationNights { get; set; }

        [Range(0, double.MaxValue)]
        public int ChildPrice { get; set; }

        [Range(0, double.MaxValue)]
        public int AdultPrice { get; set; }

        public string? ThumbnailImageUrl { get; set; }

        [Range(0, 5)]
        public decimal? AverageRating { get; set; }

        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Min Group Size must be a positive number.")]
        public int? MinGroupSize { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max Group Size must be a positive number.")]
        public int? MaxGroupSize { get; set; }
        public string? DeparturePoints { get; set; }
        public string? IncludedServices { get; set; }
        public string? ExcludedServices { get; set; }
        public string? ImageUrls { get; set; }

        public bool IsCancellable { get; set; }
        public string? CancellationPolicyDescription { get; set; }
        public decimal? SumRating { get; set; }
    }
}
