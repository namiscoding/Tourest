namespace Tourest.Data.Entities
{
	public class TourCategory
	{
		public int TourID { get; set; } // PK part & FK property
		public int CategoryID { get; set; } // PK part & FK property

		// Navigation Properties
		public virtual Tour Tour { get; set; } = null!;
		public virtual Category Category { get; set; } = null!;
	}
}
