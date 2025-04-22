namespace Tourest.ViewModels.UserView
{
    public class UserViewModel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty; // Hoặc UserName, Email tùy bạn muốn hiển thị gì
        public string? ProfilePictureUrl { get; set; }
    }
}
