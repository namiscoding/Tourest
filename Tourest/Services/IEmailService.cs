using Tourest.Data.Entities;

namespace Tourest.Services
{
    public interface IEmailService    {

        bool SendEmail(string toEmail, string subject, string htmlBody);
    }
}
