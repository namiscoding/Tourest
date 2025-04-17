namespace Tourest.Models
{
	public class Account
	{
		public int AccountID { get; set; }
		public int UserID { get; set; } // Foreign Key Property
		public string Username { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public DateTime? LastLoginDate { get; set; }
		public string? PasswordResetToken { get; set; }
		public DateTime? ResetTokenExpiration { get; set; }

		// Navigation Property
		public virtual User User { get; set; } = null!;
	}
}
