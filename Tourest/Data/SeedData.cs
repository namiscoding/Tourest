using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities; 
using Microsoft.AspNetCore.Identity;
using Tourest.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>()) // Thay TourestDbContext bằng DbContext của bạn
        {
           
            // Kiểm tra xem đã có dữ liệu chưa
            if (context.Users.Any() || context.Accounts.Any())
            {
                return; // Cơ sở dữ liệu đã được seed
            }

            // Tạo một User mới
            var adminUser = new User
            {
                FullName = "Tourest Admin",
                Email = "admin@tourest.com",
                PhoneNumber = "1234567890",
                Address = "123 Admin St",
                RegistrationDate = DateTime.UtcNow,
                IsActive = true,
            };

            // Tạo một Account mới và liên kết với User
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
            var adminAccount = new Account
            {
                Username = "admin",
                PasswordHash = hashedPassword, // Lưu trữ mật khẩu đã hash
                Role = "Admin",
                LastLoginDate = null, // Giá trị mặc định
            };

            adminUser.Account = adminAccount; // Thiết lập mối quan hệ

            // Thêm User và Account vào DbContext
            context.Users.Add(adminUser);
            context.Accounts.Add(adminAccount); // Không cần thiết phải add account vì đã add user và account có quan hệ

            // Lưu các thay đổi vào cơ sở dữ liệu
            await context.SaveChangesAsync();
        }
    }
}
