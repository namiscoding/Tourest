using Tourest.Data.Entities;

namespace Tourest.ViewModels.Account
{
    public class AccountViewModel
    {
        public int AccountID { get; set; }
        public int UserID { get; set; } // Foreign Key Property
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; 
        public virtual UserViewModel User { get; set; } = null!;
    }
}
