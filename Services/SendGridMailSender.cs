using DecidioTestExcersice.Contracts;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace DecidioTestExcersice.Services
{
    public class SendGridMailSender: IMailSender
    {
        private readonly string apiKey;
        private readonly EmailAddress from;

        public SendGridMailSender(string apiKey, MailAddress from)
        {
            this.apiKey = apiKey;
            this.from = new EmailAddress { Email = from.Address, Name = from.User };
        }

        public async Task SendEmailAsync(MailAddress to, string subject, string body)
        {
            var client = new SendGridClient(this.apiKey);
            var message = new SendGridMessage
            {
                Subject = subject,
                From = from,
                PlainTextContent = body,
            };

            Response response = await client.SendEmailAsync(message);
        }
    }
}
