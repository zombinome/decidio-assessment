using DecidioTestExcersice.Contracts;
using DecidioTestExcersice.Errors;
using DecidioTestExcersice.Models;
using DecidioTestExcersice.Repositiories;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DecidioTestExcersice.Services
{
    public class MailService
    {
        public const string VarUserName = "userName";
        public const string VarEmail = "email";

        private static readonly Regex templateRegEx = new Regex(@"{\{(\w[\w\d]*)\}\}", RegexOptions.Compiled | RegexOptions.Multiline);

        private readonly IMailSender mailSender;
        private readonly MailAddressRepository mailAddressRepository;

        public MailService(IMailSender mailSender, MailAddressRepository mailAddressRepository)
        {
            this.mailSender = mailSender;
            this.mailAddressRepository = mailAddressRepository;
        }

        public async Task SendMailsAsync(IEnumerable<string> emails, MailTemplate template)
        {
            IEnumerable<MailAddress> addresses;
            try
            {
                addresses = emails
                    .Select(email => new MailAddress(email.Replace("\"", string.Empty)))
                    .ToList();
            }
            catch (FormatException ex)
            {
                throw new InvalidMailAddressException("At least one of email addresses provided is not valid email address", ex);
            }

            addresses = await FilterByLastSentTime(addresses).ConfigureAwait(false);
            
            foreach (var mailAddress in addresses)
            {
                string userName = string.IsNullOrEmpty(mailAddress.DisplayName)
                    ? mailAddress.Address
                    : mailAddress.DisplayName;

                var variables = new Dictionary<string, string>()
                {
                    [VarUserName] = userName,
                    [VarEmail] = mailAddress.Address,
                };

                string subject = ProcessTemplate(template.Subject, variables);
                string body = ProcessTemplate(template.Body, variables);

                await this.mailSender.SendEmailAsync(mailAddress, subject, body);
                await this.mailAddressRepository.UpdateMailEntryTimestamp(mailAddress.Address);
            }
        }

        private async Task<IEnumerable<MailAddress>> FilterByLastSentTime(IEnumerable<MailAddress> addresses)
        {
            IEnumerable<string> mailsToCheck = addresses
                .Select(address => address.Address);

            IEnumerable<MailAddressEntry> knownAddresses = await this.mailAddressRepository.GetEntriesByEmail(mailsToCheck)
                .ConfigureAwait(false);

            // Filtering mails that are exist in database and were updated recently
            var cutoff = DateTime.UtcNow.AddMinutes(-5);
            cutoff = DateTime.SpecifyKind(cutoff, DateTimeKind.Unspecified); // Hack to fix DateTimeKind

            var result = new List<MailAddress>();
            foreach(var address in addresses)
            {
                bool isRecent = knownAddresses
                    .Any(ka => string.Equals(ka.Email, address.Address, StringComparison.OrdinalIgnoreCase) && ka.LastSent >= cutoff);
                if (isRecent)
                {
                    continue;
                }

                result.Add(address);
            }

            return result;
        }

        public static string ProcessTemplate(string templateString, IReadOnlyDictionary<string, string> variables)
        {
            return templateRegEx.Replace(templateString, (Match match) =>
            {
                var variableName = match.Groups[1].Value;
                if (variables.ContainsKey(variableName))
                {
                    return variables[variableName];
                }
                return string.Empty;
            });
        }
    }
}
