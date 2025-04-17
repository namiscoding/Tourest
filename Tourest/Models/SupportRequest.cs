namespace Tourest.Models
{
	public class SupportRequest
	{
		public int RequestID { get; set; }
		public int CustomerID { get; set; } // FK property
		public string Subject { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public DateTime SubmissionDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public int? HandlerUserID { get; set; } // FK property (nullable)
		public string? ResolutionNotes { get; set; }

		// Navigation Properties
		public virtual User Customer { get; set; } = null!;
		public virtual User? HandlerUser { get; set; }
	}
}
