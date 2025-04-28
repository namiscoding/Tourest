using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin.AdminTour
{
    public class AdminTourDetailsViewModel
    {
        public int TourID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationDays { get; set; }
        public int DurationNights { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int AdultPrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ChildPrice { get; set; }
        public int? MinGroupSize { get; set; }
        public int? MaxGroupSize { get; set; }
        public string? DeparturePoints { get; set; }
        public string? IncludedServices { get; set; }
        public string? ExcludedServices { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? AverageRating { get; set; }
        public bool IsCancellable { get; set; }
        public string? CancellationPolicyDescription { get; set; }

        // Dữ liệu liên quan đã xử lý
        public List<string> CategoryNames { get; set; } = new();
        public List<AdminItineraryDayViewModel> ItineraryDays { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new(); // Danh sách URL đầy đủ
    }
}
