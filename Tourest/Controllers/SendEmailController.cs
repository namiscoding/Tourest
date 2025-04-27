using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.Util;

namespace Tourest.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class SendEmailController : Controller
    {
        private readonly IEmailService _emailserivce;

        public SendEmailController(IEmailService emailserivce)
        {
            _emailserivce = emailserivce;
        }

        [HttpPost("send")]
        public IActionResult Index()

        {
            EmailRequest request = new EmailRequest();
            //var sampleTour = new Tour
            //{
            //    TourID = 1,
            //    Name = "Khám phá Đà Lạt 3N2Đ",
            //    Destination = "Đà Lạt",
            //    Description = "Chuyến đi tuyệt vời đến thành phố ngàn hoa.",
            //    DurationDays = 3,
            //    DurationNights = 2,
            //    AdultPrice = 3500000,
            //    ChildPrice = 2500000,
            //    MinGroupSize = 5,
            //    MaxGroupSize = 20,
            //    DeparturePoints = "Hà Nội, TP.HCM",
            //    IncludedServices = "Khách sạn, ăn uống, vé tham quan",
            //    ExcludedServices = "Chi phí cá nhân",
            //    ImageUrls = "https://example.com/image1.jpg;https://example.com/image2.jpg",
            //    Status = "Active",
            //    AverageRating = 4.7M,
            //    IsCancellable = true,
            //    CancellationPolicyDescription = "Hủy trước 7 ngày được hoàn tiền 100%"
            //};
            //request.htmlbody = MailUtil.CreateBooking(sampleTour);
            request.ToEmail = "nguyenducanhqbz@gmail.com";
            request.Subject = "Ducanh dz _ DIt me cac em";
            bool result = _emailserivce.SendEmail(request.ToEmail, request.Subject, request.htmlbody);

            if (result)
                return Ok(new { message = "Email sent successfully!" });
            else
                return BadRequest(new { message = "Failed to send email." });
        }


        public class EmailRequest
        {
            public string ToEmail { get; set; }
            public string Subject { get; set; }
            public string htmlbody { get; set; }
    
        }
    }
}
