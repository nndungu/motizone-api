using Castle.Core.Smtp;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MotiZone.Data;
using MotiZone.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace MotiZone.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly PdfInvoiceService _pdfInvoiceService;
    private readonly SmtpEmailSender _smtpEmailSender;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public PaymentService(ApplicationDbContext context, PdfInvoiceService pdfInvoiceService, SmtpEmailSender smtpEmailSender,
        UserManager<User> userManager, IConfiguration configuration)
    {
        _context = context;
        _pdfInvoiceService = pdfInvoiceService;
        _smtpEmailSender = smtpEmailSender;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Payment> CreatePaymentAsync(int userId, decimal amount, string method = "Card")
    {
        var payment = new Payment
        {
            UserId = userId,
            Amount = amount,
            PaidAt = DateTime.UtcNow,
            PaymentMethod = method
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<List<Payment>> GetPaymentsByUserAsync(int userId)
    {
        return await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();
    }

    private async Task SendReceiptAsync(Order order)
    {
        var user = await _userManager.FindByIdAsync(order.UserId.ToString());
        if (user == null || string.IsNullOrEmpty(user.Email)) return;
        
        var pdfService = new PdfInvoiceService();
           var pdfBytes = pdfService.GenerateInvoice(order);
           
           await _smtpEmailSender.SendEmailAsync(
               user.Email,
               "MotiZone Order Confirmation",
               $"<h3>Your Order #{order.Id} has been placed.</h3><p>Total: KES {order.TotalAmount}</p>",
               pdfBytes,
               $"Invoice_{order.Id}.pdf"
           );
           
           
        
        var pdf = _pdfInvoiceService.GenerateInvoice(order);
        var builder = new BodyBuilder
        {
            HtmlBody = $"<p>Thanks for your order #{order.Id}.</p><p>Amount: KES {order.TotalAmount}</p>"
        };
        builder.Attachments.Add($"Invoice_{order.Id}.pdf", pdf, ContentType.Parse("application/pdf"));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MotiZone", _configuration["Email:From"]));
        message.To.Add(MailboxAddress.Parse(user.Email));
        message.Subject = $"Invoice for Order #{order.Id}";
        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_configuration["Email:Smtp"], 587, false);
        await smtp.AuthenticateAsync(_configuration["Email:User"], _configuration["Email:Password"]);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);

    }
    public async Task<List<Payment>> GetAllPaymentsAsync(
        string method = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? userId = null)
    {
        var query = _context.Payments.AsQueryable();

        if (!string.IsNullOrEmpty(method))
            query = query.Where(p => p.PaymentMethod == method);

        if (startDate.HasValue)
            query = query.Where(p => p.PaidAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(p => p.PaidAt <= endDate.Value);

        if (userId.HasValue)
            query = query.Where(p => p.UserId == userId.Value);

        return await query.OrderByDescending(p => p.PaidAt).ToListAsync();
    }


}