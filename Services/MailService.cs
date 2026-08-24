using DengeWeb.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DengeWeb.Services;

public class MailService : IMailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<MailService> _logger;

    public MailService(IConfiguration config, ILogger<MailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(ContactViewModel request)
    {
        var settings = GetSmtpSettings();
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Denge Defence İletişim", settings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(settings.ReceiverEmail));
        email.ReplyTo.Add(new MailboxAddress(request.FullName, request.Email));
        email.Subject = $"Yeni Mesaj: {request.Subject}";

        var builder = new BodyBuilder
        {
            HtmlBody = $"<b>Gönderen:</b> {request.FullName} <br> <b>E-Posta:</b> {request.Email} <br> <b>Telefon:</b> {request.Phone} <br><br> <b>Mesaj:</b><br> {request.Message}"
        };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(settings.Server, settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(settings.SenderEmail, settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendPasswordResetEmailAsync(string recipient, string resetUrl)
    {
        var settings = GetSmtpSettings();
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Denge Defence", settings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(recipient));
        email.Subject = "Yönetici şifre sıfırlama";
        email.Body = new BodyBuilder { HtmlBody = $"Şifrenizi sıfırlamak için <a href=\"{resetUrl}\">bu bağlantıyı</a> kullanın. Bağlantı 30 dakika geçerlidir." }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(settings.Server, settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(settings.SenderEmail, settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    private SmtpSettings GetSmtpSettings()
    {
        var server = _config["SmtpSettings:Server"];
        var portText = _config["SmtpSettings:Port"];
        var sender = _config["SmtpSettings:SenderEmail"];
        var password = _config["SmtpSettings:Password"];
        var receiver = _config["SmtpSettings:ReceiverEmail"];

        if (string.IsNullOrWhiteSpace(server) || !int.TryParse(portText, out var port) ||
            string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(receiver))
        {
            throw new InvalidOperationException("SmtpSettings eksik veya geçersiz.");
        }

        return new SmtpSettings(server, port, sender, password, receiver);
    }

    private sealed record SmtpSettings(string Server, int Port, string SenderEmail, string Password, string ReceiverEmail);
}