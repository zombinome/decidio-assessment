using DecidioTestExcersice.Contracts;
using DecidioTestExcersice.Services;
using System.Net.Mail;

namespace DecidioTestExcersice.Initialization
{
    internal static class SendGridRegistration
    {
        internal static IServiceCollection AddSendGridMailSender(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<IMailSender, SendGridMailSender>(serviceProvider =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var apiKey = config.GetValue<string>("SendGrid:ApiKey");
                var fromField = config.GetValue<string>("SendGrid:From");
                var from = new MailAddress(fromField);

                return new SendGridMailSender(apiKey, from);
            });
    }
}
