namespace Tourest.Data.Entities
{
	public class Rating
	{
		public int RatingID { get; set; }
		public int CustomerID { get; set; } 
		public decimal RatingValue { get; set; }
		public string? Comment { get; set; }
		public DateTime RatingDate { get; set; }
		public string RatingType { get; set; } = string.Empty; 

		
		public virtual User Customer { get; set; } = null!;
		
		public virtual TourRating? TourRating { get; set; }
		public virtual TourGuideRating? TourGuideRating { get; set; }
	}

}
