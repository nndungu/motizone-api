using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services;

public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private readonly PdfInvoiceService _pdfInvoiceService;
    private readonly IConfiguration _configuration;

    public OrderService(ApplicationDbContext context, CartService cartService, PdfInvoiceService pdfInvoiceService, IConfiguration configuration)
    {
        _context = context;
        _cartService = cartService;
        _pdfInvoiceService = pdfInvoiceService;
        _configuration = _configuration;
    }
    /*public async Task<Order?> CheckoutAsync(int userId)
    {
        var orderId = await _cartService.CheckoutAsync(userId);
        return orderId.HasValue ? await _context.Orders.FindAsync(orderId.Value) : null;
        
    }*/
    public async Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

      //  order.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<Order> CheckoutAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Include(ci => ci.Vehicle)
            .Include(ci => ci.Part)
            .Include(ci => ci.Customization)
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any()) throw new Exception("Cart is empty");

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = cartItems.Sum(i =>
                i.Vehicle?.Price ?? i.Part?.Price ?? i.Customization?.Price ?? 0)
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                VehicleId = item.VehicleId,
                PartId = item.PartId,
                CustomizationId = item.CustomizationId,
                Quantity = item.Quantity,
                Price = item.Vehicle?.Price ?? item.Part?.Price ?? item.Customization?.Price ?? 0
            });
        }

        await _context.SaveChangesAsync();
        await _cartService.ClearCartAsync(userId);

        // 🔔 Send Email with PDF Invoice
        var pdf = _pdfInvoiceService.GenerateInvoice(order);
        var builder = new BodyBuilder
        {
            HtmlBody = $"<p>Thanks for your order #{order.Id}.</p><p>Amount: KES {order.TotalAmount}</p>"
        };
        builder.Attachments.Add($"Invoice_{order.Id}.pdf", pdf, ContentType.Parse("application/pdf"));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MotiZone", _configuration["Email:From"]));
       // message.To.Add(MailboxAddress.Parse(User.Email));
        message.Subject = $"Invoice for Order #{order.Id}";
        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
      //  await smtp.ConnectAsync(_configuration["Email:Smtp"], 587, false);
       // await smtp.AuthenticateAsync(_configuration["Email:User"], _configuration["Email:Password"]);
       // await SmtpClient.SendAsync(message);
        //await smtp.DisconnectAsync(true);


        return order;
    }


}