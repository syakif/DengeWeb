using DengeWeb.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DengeWeb.Services;

public class MailService : IMailService
{
    private readonly IConfiguration _config;
    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(ContactViewModel request)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Denge Defence İletişim", _config["SmtpSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(_config["SmtpSettings:ReceiverEmail"]));
        email.ReplyTo.Add(new MailboxAddress(request.FullName, request.Email));
        email.Subject = $"Yeni Mesaj: {request.Subject}";

        var builder = new BodyBuilder
        {
            HtmlBody = $"<b>Gönderen:</b> {request.FullName} <br> <b>E-Posta:</b> {request.Email} <br><br> <b>Mesaj:</b><br> {request.Message}"
        };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"]), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["SmtpSettings:SenderEmail"], _config["SmtpSettings:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}