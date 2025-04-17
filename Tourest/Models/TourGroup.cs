namespace Tourest.Models
{
	public class TourGroup
	{
		public int TourGroupID { get; set; }
		public int TourID { get; set; } // FK property
		public DateTime DepartureDate { get; set; } // Dùng DateTime, cấu hình thành DATE
		public int TotalGuests { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreationDate { get; set; }
		public int? AssignedTourGuideID { get; set; } // FK property

		// Navigation Properties
		public virtual Tour Tour { get; set; } = null!;
		public virtual User? AssignedTourGuide { get; set; }
		public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
		public virtual ICollection<TourGuideAssignment> TourGuideAssignments { get; set; } = new List<TourGuideAssignment>();
		public virtual ICollection<TourGuideRating> TourGuideRatings { get; set; } = new List<TourGuideRating>();
	}
}
