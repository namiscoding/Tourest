using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tourest.Data.Entities
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
        public override string ToString()
        {
            return $"AccountID: {AccountID}, Username: {Username}, Role: {Role}, " +
                   $"UserID: {UserID}, UserName: {(User != null ? User.FullName : "null")}, " +
                   $"LastLogin: {(LastLoginDate.HasValue ? LastLoginDate.Value.ToString("yyyy-MM-dd HH:mm") : "N/A")}, " +
                   $"ResetTokenExpires: {(ResetTokenExpiration.HasValue ? ResetTokenExpiration.Value.ToString("yyyy-MM-dd HH:mm") : "N/A")}";
        }

        // Navigation Property
        public virtual User User { get; set; } = null!;


	}
}
