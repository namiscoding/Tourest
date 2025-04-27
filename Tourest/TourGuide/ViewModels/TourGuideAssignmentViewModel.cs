using Tourest.Data.Entities;

namespace Tourest.TourGuide.ViewModels
{
    public class TourGuideAssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public int TourGuideId { get; set; }
        public string TourGroupName { get; set; }
        public DateTime DepartureDate { get; set; }
        public string PickupPoint { get; set; }
        public int TotalAdults { get; set; }
        public int TotalChildren { get; set; }
        public string TourName { get; set; }
        public string Status { get; set; }
        public DateTime AssignmentDate { get; set; }
        public List<Rating> TourRating { get; set; }
        public double TourGuideRating { get; set; }
        public double Destination {  get; set; }
        public DateTime Deadline { get; set; }

    }
}
