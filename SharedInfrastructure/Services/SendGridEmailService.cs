using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SharedInfrastructure.Services
{
    public class SendGridEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        public SendGridEmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGridSettings:ApiKey"];
            _fromEmail = configuration["SendGridSettings:FromEmail"];
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
