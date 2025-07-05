using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace MotiZone.Services;

public class EmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtp = _config.GetSection("Smtp");
        var client = new SmtpClient
        {
            Host = smtp["Host"],
            Port = int.Parse(smtp["Port"]),
            EnableSsl = bool.Parse(smtp["EnableSsl"]),
            Credentials = new NetworkCredential(smtp["User"], smtp["Password"])
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mail.To.Add(email);
        return client.SendMailAsync(mail);
    }
}