namespace Tourest.Models
{
	public class Rating
	{
		public int RatingID { get; set; }
		public int CustomerID { get; set; } // FK property
		public decimal RatingValue { get; set; }
		public string? Comment { get; set; }
		public DateTime RatingDate { get; set; }
		public string RatingType { get; set; } = string.Empty; // Discriminator handled implicitly by TPT mapping

		// Navigation Property
		public virtual User Customer { get; set; } = null!;
		// Properties for derived types
		public virtual TourRating? TourRating { get; set; }
		public virtual TourGuideRating? TourGuideRating { get; set; }
	}

}
