namespace Tourest.ViewModels
{
   
    public class UnassignedBookingViewModel
    {
        public int BookingId { get; set; }
        public int TourGroupId { get; set; }
        public string TourName { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public string PickupPoint { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int TotalGuests => NumberOfAdults + NumberOfChildren;
        public int TourId { get; set; }
    }

    public class RecommendedGuideViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public string Languages { get; set; }
        public int Experience { get; set; }
        public int MaxCapacity { get; set; }
        public string Specializations { get; set; }
        public double SuitabilityScore { get; set; }
        public int TourDifficulty { get; set; } 
        public int RequiredExperience { get; set; }
    }
  
}
