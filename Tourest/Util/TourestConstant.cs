namespace Tourest.Util
{
    public class TourestConstant
    {
        public const string CompanyName = "Tourest";

        public const string TemplateBooking = @"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <title>Xác nhận đặt tour - Tourest</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
    <div style=""max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 8px;"">
        <h2 style=""color: #2E86C1;"">Xin chào, {{CustomerName}}!</h2>

        <p>Cảm ơn bạn đã đặt tour tại <strong>Tourest</strong>. Chúng tôi rất hân hạnh được đồng hành cùng bạn trong chuyến đi sắp tới.</p>

        <h3 style=""color: #117A65;"">Thông tin đơn hàng:</h3>
        <table style=""width: 100%; border-collapse: collapse;"">
            <tr>
                <td style=""padding: 8px; border: 1px solid #ddd;""><strong>Tên Tour:</strong></td>
                <td style=""padding: 8px; border: 1px solid #ddd;"">{{TourName}}</td>
            </tr>
            <tr>
                <td style=""padding: 8px; border: 1px solid #ddd;""><strong>Ngày khởi hành:</strong></td>
                <td style=""padding: 8px; border: 1px solid #ddd;"">{{StartDate}}</td>
            </tr>
            <tr>
                <td style=""padding: 8px; border: 1px solid #ddd;""><strong>Số lượng khách:</strong></td>
                <td style=""padding: 8px; border: 1px solid #ddd;"">{{GuestCount}}</td>
            </tr>
            <tr>
                <td style=""padding: 8px; border: 1px solid #ddd;""><strong>Tổng tiền:</strong></td>
                <td style=""padding: 8px; border: 1px solid #ddd; color: #E74C3C;""><strong>{{TotalAmount}} VND</strong></td>
            </tr>
        </table>

        <p style=""margin-top: 20px;"">Chúng tôi sẽ liên hệ với bạn để xác nhận chi tiết và hướng dẫn thanh toán nếu cần.</p>

        <p>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ hotline: <strong>1900 9999</strong> hoặc email: <a href=""mailto:support@tourest.com"">support@tourest.com</a>.</p>

        <div style=""text-align: center; margin-top: 30px;"">
            <a href=""https://tourest.com"" style=""background-color: #3498DB; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">
                Xem chi tiết đơn hàng
            </a>
        </div>

        <p style=""margin-top: 30px; font-size: 12px; color: #888888;"">© 2025 Tourest. All rights reserved.</p>
    </div>
</body>
</html>
";
    }
}
