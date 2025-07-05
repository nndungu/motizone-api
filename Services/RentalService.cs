using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services;

public class RentalService
{
    private readonly ApplicationDbContext _context;

    public RentalService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRentalAsync(Rental rental)
    {
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync(); }

    public async Task<List<Rental>> GetAllRentalsAsync()
    {
        return await _context.Rentals.Include(r => r.Vehicle).ToListAsync();
    }

    public async Task<Rental?> GetRentalByIdAsync(int id)
    {
        return await _context.Rentals.Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> BookRentalAsync(Rental rental)
    {
        rental.Status = "Booked";
        rental.StartDate = DateTime.UtcNow;

        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdateRentalAsync(Rental rental)
    {
        var existing = await _context.Rentals.FindAsync(rental.Id);
        if (existing == null) return false;

        existing.VehicleId = rental.VehicleId;
        existing.TotalCost = rental.TotalCost;
        existing.DurationDays = rental.DurationDays;
        existing.Status = rental.Status;

        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<bool> CancelRentalAsync(int rentalId)
    {
        var rental = await _context.Rentals.FindAsync(rentalId);
        if (rental == null) return false;

        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();
        return true;
    }
}