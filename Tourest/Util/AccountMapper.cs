using Tourest.Data.Entities;
using Tourest.ViewModels.Account;
using Tourest.ViewModels.Admin;

namespace Tourest.Util
{
    public class AccountMapper
    {
        public static Account RegisterModeltoAccount(RegisterModel model, User user)
        {
            return new Account
            {
                Username = model.Email,
                PasswordHash = model.PasswordHash, // Nếu đã băm rồi. Nếu chưa thì nên băm ở đây.
                Role = "Customer",                     // Gán mặc định vai trò nếu chưa có input
                UserID = user.UserID,
                User = user,
                LastLoginDate = null,
                PasswordResetToken = null,
                ResetTokenExpiration = null
            };
        }

        public static User RegisterModelToUser(RegisterModel model)
        {
            return new User
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.Fullname,               // Không có trong RegisterModel
                Address = null,                // Không có trong RegisterModel
                ProfilePictureUrl = null,      // Không có trong RegisterModel
                RegistrationDate = DateTime.UtcNow, // Mặc định khi đăng ký
                IsActive = true                // Mặc định người dùng mới là active
            };
        }
        public static UserViewModel ToUserViewModel(User userInsert, Account accountInsert)
        {
            return new UserViewModel
            {
                UserID = userInsert.UserID,
                FullName = userInsert.FullName,
                Email = userInsert.Email,
                PhoneNumber = userInsert.PhoneNumber,
                Address = userInsert.Address,
                ProfilePictureUrl = userInsert.ProfilePictureUrl,
                RegistrationDate = userInsert.RegistrationDate,
                IsActive = userInsert.IsActive,
                Account = new AccountViewModel
                {
                    AccountID = accountInsert.AccountID,
                    UserID = accountInsert.UserID,
                    Username = accountInsert.Username,
                    Role = accountInsert.Role,

                }
                // Các thuộc tính điều hướng khác có thể được ánh xạ tương tự nếu cần
            };
        }
        public static UserViewModel UserToUserViewModel(User userInsert)
        {
            Console.WriteLine("---- UserViewModel ----");
            Console.WriteLine($"UserID: {userInsert.UserID}");
            Console.WriteLine($"FullName: {userInsert.FullName}");
            Console.WriteLine($"Email: {userInsert.Email}");
            Console.WriteLine($"PhoneNumber: {userInsert.PhoneNumber}");
            Console.WriteLine($"Address: {userInsert.Address}");
            Console.WriteLine($"ProfilePictureUrl: {userInsert.ProfilePictureUrl}");
            Console.WriteLine($"RegistrationDate: {userInsert.RegistrationDate}");
            Console.WriteLine($"IsActive: {userInsert.IsActive}");


            Console.WriteLine("--- Account ---");
            Console.WriteLine($"AccountID: {userInsert.Account.AccountID}");
            Console.WriteLine($"UserID: {userInsert.Account.UserID}");
            Console.WriteLine($"Username: {userInsert.Account.Username}");
            Console.WriteLine($"Role: {userInsert.Account.Role}");


            return new UserViewModel
            {
                UserID = userInsert.UserID,
                FullName = userInsert.FullName,
                Email = userInsert.Email,
                PhoneNumber = userInsert.PhoneNumber,
                Address = userInsert.Address,
                ProfilePictureUrl = userInsert.ProfilePictureUrl,
                RegistrationDate = userInsert.RegistrationDate,
                IsActive = userInsert.IsActive,
                Account = new AccountViewModel
                {
                    AccountID = userInsert.Account.AccountID,
                    UserID = userInsert.Account.UserID,
                    Username = userInsert.Account.Username,
                    Role = userInsert.Account.Role,

                }
                // Các thuộc tính điều hướng khác có thể được ánh xạ tương tự nếu cần
            };


        }
        public static EditCustomerViewModel ConvertoEdit(UserViewModel userViewModel)
        {
            return new EditCustomerViewModel
            {
                UserId = userViewModel.UserID,
                FullName = userViewModel.FullName,
                Email = userViewModel.Email,
                Address = userViewModel.Address,
                IsActive = userViewModel.IsActive,
                PhoneNumber = userViewModel.PhoneNumber

            };

        }
    }
}
