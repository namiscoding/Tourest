namespace Tourest.Models
{
	public class TourGuideAssignment
	{
		public int AssignmentID { get; set; }
		public int TourGroupID { get; set; } // FK property
		public int TourGuideID { get; set; } // FK property (User ID of guide)
		public int TourManagerID { get; set; } // FK property (User ID of manager)
		public DateTime AssignmentDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public string? RejectionReason { get; set; }
		public DateTime? ConfirmationDate { get; set; }

		// Navigation Properties
		public virtual TourGroup TourGroup { get; set; } = null!;
		public virtual User TourGuide { get; set; } = null!;
		public virtual User TourManager { get; set; } = null!;
	}
}
