namespace Tourest.Models
{
	public class TourGuide
	{
		public int TourGuideUserID { get; set; } // PK & FK Property
		public string? ExperienceLevel { get; set; }
		public string? LanguagesSpoken { get; set; }
		public string? Specializations { get; set; }
		public int? MaxGroupSizeCapacity { get; set; }

		// [Column(TypeName = "decimal(3, 2)")] // Có thể cấu hình = Fluent API thay thế
		public decimal? AverageRating { get; set; }

		// Navigation Property
		public virtual User User { get; set; } = null!;
	}
}
