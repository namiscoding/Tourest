namespace Tourest.Data.Entities
{
	public class TourRating 
	{
		public int RatingID { get; set; } 
		public int TourID { get; set; } 

		// Navigation Properties
		public virtual Rating Rating { get; set; } = null!;
		public virtual Tour Tour { get; set; } = null!;
	}
}
