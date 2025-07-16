/*using Microsoft.AspNetCore.Identity;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Models.ViewModels;

namespace MotiZone.Services;

public class AdminPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public AdminPaymentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        //_userManager = userManager;
    }

    /*public async Task<List<AdminPaymentViewModel>> GetAllPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.User)
            .Select(p => new AdminPaymentViewModel
            {
                Id = p.Id,
                Method = p.Method,
                Status = p.Status,
                Reference = p.Method == "Mpesa" ? p.TransactionCode : p.StripeSessionId,
                Amount = p.Amount,
                Date = p.Order.OrderDate,
                UserEmail = p.Order.User.Email
            }).ToListAsync();
    }#1#
}*/