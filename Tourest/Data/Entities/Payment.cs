namespace Tourest.Data.Entities
{
	public class Payment
	{
		public int PaymentID { get; set; }
		public int BookingID { get; set; } // FK property
		public int Amount { get; set; }
		public DateTime PaymentDate { get; set; }
		public string PaymentMethod { get; set; } = string.Empty;
		public string? TransactionID { get; set; }
		public string Status { get; set; } = string.Empty;

		// Navigation Property
		public virtual Booking Booking { get; set; } = null!;
	}
}
