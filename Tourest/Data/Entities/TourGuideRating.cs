namespace Tourest.Data.Entities
{
	public class TourGuideRating // Can inherit Rating
	{
		public int RatingID { get; set; } // PK & FK property
		public int TourGuideID { get; set; } // FK property (User ID)
		public int TourGroupID { get; set; } // FK property

		// Navigation Properties
		public virtual Rating Rating { get; set; } = null!;
		public virtual User TourGuide { get; set; } = null!;
		public virtual TourGroup TourGroup { get; set; } = null!;
	}
}
