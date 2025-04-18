namespace Tourest.Data.Entities
{
	public class TourAuditLog
	{
		public long AuditLogID { get; set; } // Use long for potentially many logs
		public int TourID { get; set; } // FK property
		public string ActionType { get; set; } = string.Empty;
		public int PerformedByUserID { get; set; } // FK property
		public DateTime Timestamp { get; set; }
		public string? OldValues { get; set; } // JSON stored as string
		public string? NewValues { get; set; } // JSON stored as string

		// Navigation Properties
		public virtual Tour Tour { get; set; } = null!;
		public virtual User PerformedBy { get; set; } = null!;
	}
}
