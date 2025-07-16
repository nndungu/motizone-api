/*using MailKit.Net.Smtp;
using MimeKit;
using MotiZone.Models;

namespace MotiZone.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOrderConfirmationAsync(Order order, byte[]? invoicePdf = null)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MotiZone", _config["Email:SmtpServer:From"]));
        message.To.Add(MailboxAddress.Parse(order.User?.Email ?? throw new InvalidOperationException("User email missing")));
        message.Subject = $"Order Confirmation - #{order.Id}";

        var builder = new BodyBuilder
        {
            HtmlBody = $"<h2>Thank you for your order #{order.Id}</h2>" +
                       $"<p>Total: KES {order.TotalAmount}</p>" +
                       $"<p>We’ll send you another update when your items ship.</p>"
        };

        if (invoicePdf != null)
            builder.Attachments.Add($"Invoice_{order.Id}.pdf", invoicePdf, new ContentType("application", "pdf"));

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["Email:SmtpServer:Host"], int.Parse(_config["Email:SmtpServer:Port"]), true);
        await client.AuthenticateAsync(_config["Email:SmtpServer:Username"], _config["Email:SmtpServer:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}*/