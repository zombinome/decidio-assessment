using System.Net.Mail;

namespace DecidioTestExcersice.Contracts
{
    public interface IMailSender
    {
        public Task SendEmailAsync(MailAddress to, string subject, string body);
    }
}
