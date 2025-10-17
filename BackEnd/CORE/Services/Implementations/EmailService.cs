using CORE.Abstractions;
using CORE.Resources;
using CORE.Services.Abstractions;

namespace CORE.Services.Implementations;

public class EmailService : IEmailService
{
    private IEmailSender _emailSender { get; set; }

    public EmailService(IEmailSender sender)
    {
        _emailSender = sender;
    }

    public async Task SendWelcomeEmailAsync(string to, string subject, string name, string verificationLink)
    {
        var body = EmailTemplateGenerator.GenerateWelcomeTemplate(name, verificationLink);
        await _emailSender.SendAsync(to, subject, body);
    }
}
