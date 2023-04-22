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
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);
        }

        //private static void Main()
        //{
        //    Execute().Wait();
        //}

        //static async Task Execute()
        //{
        //    var apiKey = Environment.GetEnvironmentVariable("SendGridStudentKey");
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress("kiahdaulton@gmail.com", "Hall Haven");
        //    var subject = "Sending with SendGrid is Fun";
        //    var to = new EmailAddress("kiahdaulton@gmail.com", "Hall Haven User");
        //    var plainTextContent = "and easy to do anywhere, even with C#";
        //    var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    var response = await client.SendEmailAsync(msg);
        //}
    
    }
}