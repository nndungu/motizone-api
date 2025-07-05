using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services
{
    public class VehicleService
    {
        private readonly ApplicationDbContext _context;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<List<Vehicle>> GetFilteredVehiclesAsync(string? make, string? model, int? year)
        {
            var query = _context.Vehicles.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(make))
                query = query.Where(x => x.Make.Contains(make));
            if (!string.IsNullOrWhiteSpace(model))
                query = query.Where(x => x.Model.Contains(model));
            if (year.HasValue)
                query = query.Where(x => x.Year == year.Value);
            
            return await query.ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        
        public async Task<Vehicle?> UpdateVehicleAsync(Vehicle vehicle)
        {
            var existing = await _context.Vehicles.FindAsync(vehicle.Id);
            if (existing == null) return null;

            existing.Make = vehicle.Make;
            existing.Model = vehicle.Model;
            existing.Year = vehicle.Year;
            existing.Price = vehicle.Price;
            existing.ImageUrl = vehicle.ImageUrl;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
