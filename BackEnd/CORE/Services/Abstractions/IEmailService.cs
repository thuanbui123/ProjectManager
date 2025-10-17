namespace CORE.Services.Abstractions;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string to, string subject, string name, string verificationLink);
}
