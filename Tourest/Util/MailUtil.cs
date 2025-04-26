using Tourest.Data.Entities;

namespace Tourest.Util
{
    public class MailUtil
    {

        public  static string CreateBooking(Tour tour)
        {
            string emailContent = TourestConstant.TemplateBooking;

            // Thay thế các placeholder trong template
            //emailContent = emailContent.Replace("{{CustomerName}}", tour.T?? "Khách hàng");
            emailContent = emailContent.Replace("{{TourName}}", tour.Name ?? "Tour du lịch");
            //emailContent = emailContent.Replace("{{Total Day}}", tour.ItineraryDays.Count());
            //emailContent = emailContent.Replace("{{GuestCount}}", tour.GuestCount.ToString());
            //emailContent = emailContent.Replace("{{TotalAmount}}", tour.TotalAmount.ToString("N0")); // format số tiền có dấu phẩy

            return emailContent;
        }

    }
}
