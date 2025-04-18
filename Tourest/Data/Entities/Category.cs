namespace Tourest.Data.Entities
{
	public class Category
	{
		public int CategoryID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }

		// Navigation Property
		public virtual ICollection<TourCategory> TourCategories { get; set; } = new List<TourCategory>();
	}
}
