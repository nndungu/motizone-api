using MailKit.Net.Smtp;
using MimeKit;

public class SmtpEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, byte[]? attachment = null, string? filename = null)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MotiZone", _configuration["Email:SmtpServer:From"]));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };

        if (attachment != null && filename != null)
        {
            builder.Attachments.Add(filename, attachment, ContentType.Parse("application/pdf"));
        }

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_configuration["Email:SmtpServer:Host"], 587, false);
        await smtp.AuthenticateAsync(
            _configuration["Email:SmtpServer:Username"],
            _configuration["Email:SmtpServer:Password"]
        );
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}