namespace DecidioTestExcersice.Models
{
    public class MailTemplate
    {
        public string Subject { get; private set; } = string.Empty;

        public string Body { get; private set; } = string.Empty;

        public static MailTemplate Default = new MailTemplate
        {
            Subject = "Decidio Email Sending Assessment",
            Body = "Dear user, you have just received an email for {{userName}} as a part of the Decidio Email Sending Assessment development.",
        };
    }
}
