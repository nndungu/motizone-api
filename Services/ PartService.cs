using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services
{ 
    public class PartService
    {
        private readonly ApplicationDbContext _context;

        public PartService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Part>> GetPartsAsync()
        {
            return await _context.Parts.ToListAsync();
        }
        public async Task<Part?> GetPartByIdAsync(int id)
        {
            return await _context.Parts.FindAsync(id);
        }
        public async Task<Part> AddPartAsync(Part part)
        {
            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return part;
        }
        public async Task<bool> DeletePartAsync(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return false;

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Part>> GetFilteredPartsAsync(string? name, string? category)
        {
            var query = _context.Parts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{name}%"));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            return await query.ToListAsync();
        }
    }
}
