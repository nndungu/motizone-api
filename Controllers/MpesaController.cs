using Microsoft.AspNetCore.Mvc;
using MotiZone.Services;

namespace MotiZone.Controllers;

[ApiController]
[Route("payments/[action]")]
public class MpesaController : Controller
{
    private readonly MpesaService _mpesa;

    public MpesaController(MpesaService mpesa)
    {
        _mpesa = mpesa;
    }
    
    [HttpPost]
    public async Task<IActionResult> Callback()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        // Log or process M-Pesa payment
        Console.WriteLine("M-Pesa Callback Received: " + body);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Pay([FromForm] string phoneNumber, [FromForm] decimal amount)
    {
        var result = await _mpesa.InitiateStkPush(phoneNumber, amount);
        return Ok(new { result = result });
    }
}
