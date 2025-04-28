using System.Security.Cryptography;
using Tourest.Data.Entities;

namespace Tourest.Util
{
    public class MailUtil
    {

        public  static string CreateBooking(Booking book)
        {
            string emailContent = TourestConstant.TemplateBooking;

            // Thay thế các placeholder trong template
            emailContent = emailContent.Replace("{{CustomerName}}", book.Customer.FullName ?? "Quý khách hàng");
            emailContent = emailContent.Replace("{{TourName}}", book.Tour.Name ?? "Tour du lịch");
            emailContent = emailContent.Replace("{{StartDate}}", book.DepartureDate.ToString());
            emailContent = emailContent.Replace("{{GuestCount}}", (book.NumberOfAdults + book.NumberOfChildren).ToString());
            emailContent = emailContent.Replace("{{TotalAmount}}", book.TotalPrice.ToString("N0")); // format số tiền có dấu phẩy

            return emailContent;
        }
        public static string CreatEmailForgot(string token, string baseUrl = "http://localhost:5000")
        {
            string emailContent = TourestConstant.templateforgotP;

            // Tạo URL đầy đủ cho trang reset mật khẩu với token
            string resetUrl = $"{baseUrl}/Authentication/ResetPassword?token={token}";

            // Thay thế các placeholder trong template
      
           

            return resetUrl;
        }

        public static string GenerateResetToken()
        {
            Random random = new Random();
            int number = random.Next(10000, 100000); // từ 10000 đến 99999 => luôn 5 chữ số
            return number.ToString();
        }
        public static string CreatEmailForgot(string token)
        {
            string emailContent = TourestConstant.templateforgotP;

            // Thay thế các placeholder trong template
            emailContent = emailContent.Replace("{{YOUR_RESET_TOKEN}}", token ?? "Quý khách hàng");
       

            return emailContent;
        }

        public static string AssignTourGuide(Tourest.Data.Entities.TourGuide tg)
        {
            string emailContent = Tourguide.TemplateBooking;

            // Thay thế các placeholder trong template
            emailContent = emailContent.Replace("{{CustomerName}}", "Quý khách hàng");
            emailContent = emailContent.Replace("{{TGName}}", tg.User.FullName ?? "Hướng dẫn viên");
            emailContent = emailContent.Replace("{{PhoneNum}}", tg.User.PhoneNumber);
            emailContent = emailContent.Replace("{{Email}}", tg.User.Email);
            emailContent = emailContent.Replace("{{EXP}}", tg.ExperienceLevel); // format số tiền có dấu phẩy
            emailContent = emailContent.Replace("{{Language}}", tg.ExperienceLevel);
            return emailContent;
        }
    }
}
