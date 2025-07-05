using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services;

public class ServiceBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServiceBookingService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ServiceBooking>> GetBookingsAsync(int userId)
    {
        return await _context.ServiceBookings
            .Include(sb => sb.Service)
            .Where(sb => sb.UserId == userId)
            .ToListAsync();
    }

    public async Task AddBookingAsync(ServiceBooking booking)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            booking.UserId = int.Parse(userId);
        }
        _context.ServiceBookings.Add(booking);
        await _context.SaveChangesAsync();
    }
}