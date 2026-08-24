using DengeWeb.Models;

namespace DengeWeb.Services;

public interface IMailService
{
    Task SendEmailAsync(ContactViewModel request);
    Task SendPasswordResetEmailAsync(string recipient, string resetUrl);
}