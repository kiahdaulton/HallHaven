using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Configuration;

namespace HallHaven.Services
{
    public class SendStudentEmail
    {
        private readonly IConfiguration _configuration;

        public SendStudentEmail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_configuration["SendGridStudentKey"]);
            var from = new EmailAddress("kiahdaulton@gmail.com", "Hall Haven");
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = message + "<br /><br /><i>This is a message from Hall Haven. If you do not wish to receive emails, please hide your profile in your account settings.</i>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}