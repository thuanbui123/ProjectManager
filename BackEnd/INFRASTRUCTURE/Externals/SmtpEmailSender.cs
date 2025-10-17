using CORE.Models;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace INFRASTRUCTURE.Externals;

public class SmtpEmailSender : IEmailSender
{
    public async Task SendAsync(string to, string subject, string body)
    {
        var setting = new SmtpSettingModel()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            User = "thuanbui31819@gmail.com",
            From = "thuanbui31819@gmail.com",
            Password = "zvop shfr whsn znup"
        };

        var client = new SmtpClient();

        client.Connect(setting.Host, setting.Port, SecureSocketOptions.StartTls);
        client.Authenticate(setting.From, setting.Password);

        var email = new MimeMessage();
        email.Sender = new MailboxAddress(setting.DisplayName, setting.From);
        email.From.Add(new MailboxAddress(setting.DisplayName, setting.From));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = body;
        email.Body = builder.ToMessageBody();

        await client.SendAsync(email);

        client.Disconnect(true);
    }
}
