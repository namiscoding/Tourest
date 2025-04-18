namespace Tourest.Data.Entities
{
	public class ItineraryDay
	{
		public int ItineraryDayID { get; set; }
		public int TourID { get; set; } // FK property
		public int DayNumber { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int Order { get; set; }

		// Navigation Property
		public virtual Tour Tour { get; set; } = null!;
	}
}

