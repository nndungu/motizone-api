using MotiZone.Data;
using MotiZone.Models;
using Microsoft.EntityFrameworkCore;

namespace MotiZone.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> CreatePaymentAsync(int userId, decimal amount, string method = "Card")
    {
        var payment = new Payment
        {
            UserId = userId,
            Amount = amount,
            PaidAt = DateTime.Now,
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
}