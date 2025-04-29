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

        public static string AssignTourGuide()
        {
            string emailContent = Tourguide.TemplateBooking;

            // Thay thế các placeholder trong template
            emailContent = emailContent.Replace("{{CustomerName}}", "Quý khách hàng");
            emailContent = emailContent.Replace("{{TGName}}", "Hướng dẫn viên");
           ;
            return emailContent;
        }
    }
}
