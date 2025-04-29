namespace Tourest.Util
{
    public class TourestConstant
    {
        public const string CompanyName = "Tourest";

        public const string templateforgotP = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset Request</title>
    <style>
        /* Base styles */
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333333;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }
        
        /* Container */
        .email-container {
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
        }
        
        /* Header */
        .header {
            background-color: #2196F3;
            padding: 30px 0;
            text-align: center;
        }
        
        .header img {
            max-height: 60px;
        }
        
        /* Content */
        .content {
            padding: 30px 40px;
        }
        
        h1 {
            color: #333;
            font-size: 24px;
            margin-top: 0;
            margin-bottom: 20px;
        }
        
        p {
            margin-bottom: 20px;
            font-size: 16px;
        }
        
        /* Button */
        .button-container {
            text-align: center;
            margin: 30px 0;
        }
        
        .button {
            display: inline-block;
            background-color: #FF5722;
            color: #ffffff !important;
            font-weight: bold;
            text-decoration: none;
            padding: 12px 30px;
            border-radius: 4px;
            font-size: 16px;
            transition: background-color 0.3s;
        }
        
        .button:hover {
            background-color: #E64A19;
        }
        
        /* Security note */
        .security-note {
            background-color: #f3f3f3;
            padding: 15px;
            border-radius: 4px;
            font-size: 14px;
            margin-top: 20px;
            border-left: 4px solid #FFC107;
        }
        
        /* Footer */
        .footer {
            background-color: #f3f3f3;
            padding: 20px;
            text-align: center;
            font-size: 14px;
            color: #666;
        }
        
        .footer a {
            color: #2196F3;
            text-decoration: none;
        }
        
        /* Responsive */
        @media screen and (max-width: 600px) {
            .content {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <!-- Header with Logo -->
        <div class=""header"">
            <img src=""https://example.com/logo.png"" alt=""Tourest Logo"" style=""max-height: 60px;"">
        </div>
        
        <!-- Main Content -->
        <div class=""content"">
            <h1>Password Reset Request</h1>
            
            <p>Xin chào,</p>
            
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản Tourest của bạn. Nếu bạn muốn đổi mật khẩu, vui lòng nhấp vào nút bên dưới.</p>
            
            <div class=""button-container"">
                <a href=""{{YOUR_RESET_TOKEN}}"" class=""button"">Đặt lại mật khẩu</a>
            </div>
            
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với bộ phận hỗ trợ nếu bạn có thắc mắc.</p>
            
            <div class=""security-note"">
                <strong>Lưu ý bảo mật:</strong> Liên kết đặt lại mật khẩu này sẽ hết hạn sau 24 giờ. Chúng tôi không bao giờ yêu cầu mật khẩu qua email.
            </div>
        </div>
        
        <!-- Footer -->
        <div class=""footer"">
            <p>© 2025 Tourest. Tất cả các quyền được bảo lưu.</p>
            <p>
                <a href=""https://tourest.com/terms"">Điều khoản sử dụng</a> | 
                <a href=""https://tourest.com/privacy"">Chính sách bảo mật</a> | 
                <a href=""https://tourest.com/contact"">Liên hệ</a>
            </p>
            <p>Tourest Inc., 123 Đường Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh, Việt Nam</p>
        </div>
    </div>
</body>
</html>";


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
