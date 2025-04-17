namespace Tourest.Models
{
	public class TourRating // Can inherit Rating if preferred, but TPT setup in DbContext works too
	{
		public int RatingID { get; set; } // PK & FK property
		public int TourID { get; set; } // FK property

		// Navigation Properties
		public virtual Rating Rating { get; set; } = null!;
		public virtual Tour Tour { get; set; } = null!;
	}
}
