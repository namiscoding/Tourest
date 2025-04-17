namespace Tourest.Models
{
	public class Booking
	{
		public int BookingID { get; set; }
		public DateTime BookingDate { get; set; }
		public DateTime DepartureDate { get; set; } // Dùng DateTime, cấu hình thành DATE trong Fluent API
		public int NumberOfAdults { get; set; }
		public int NumberOfChildren { get; set; }
		public decimal TotalPrice { get; set; }
		public string Status { get; set; } = string.Empty;
		public string? PickupPoint { get; set; }
		public int CustomerID { get; set; } // FK property
		public int TourID { get; set; } // FK property
		public int? TourGroupID { get; set; } // FK property
		public int? PaymentID { get; set; } // FK property
		public DateTime? CancellationDate { get; set; }
		public decimal? RefundAmount { get; set; }

		// Navigation Properties
		public virtual User Customer { get; set; } = null!;
		public virtual Tour Tour { get; set; } = null!;
		public virtual TourGroup? TourGroup { get; set; }
		public virtual Payment? Payment { get; set; }
	}
}
