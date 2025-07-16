using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services;

public class MpesaLogService
{
    private readonly ApplicationDbContext _context;

    public MpesaLogService(ApplicationDbContext context)
    {
        _context = context;
    }
    /*public async Task LogCallbackAsync(MpesaCallbackResponse resp)
    {
        var log = new MpesaTransaction {
            TransactionRef = resp.Body.StkCallback.CheckoutRequestID,
            PhoneNumber = resp.Body.StkCallback.PhoneNumber,
            Amount = resp.Body.StkCallback.Amount,
            Status = resp.Body.StkCallback.ResultCode == 0 ? "Paid" : "Failed",
            Timestamp = DateTime.UtcNow
        };
        _context.MpesaTransactions.Add(log);
        await _context.SaveChangesAsync();
    }*/
}
