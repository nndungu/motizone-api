using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MotiZone.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient(_configuration["Email:SmtpServer:Host"])
        {
            Port = int.Parse(_configuration["Email:SmtpServer:Port"]),
            Credentials = new NetworkCredential(_configuration["Email:SmtpServer:Username"], _configuration["Email:SmtpServer:Password"]),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Email:SmtpServer:From"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);
        return smtpClient.SendMailAsync(mailMessage);
    }
}