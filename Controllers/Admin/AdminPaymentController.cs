using Microsoft.AspNetCore.Mvc;
using MotiZone.Services;

namespace MotiZone.Controllers.Admin;

[ApiController]
[Route("[controller]")]
[Route("Admin/[controller]/[action]")]
public class AdminPaymentsController : Controller
{
    private readonly PaymentService _paymentService;
    private readonly ExportService _exportService;

    public AdminPaymentsController(PaymentService paymentService, ExportService exportService)
    {
        _paymentService = paymentService;
        _exportService = exportService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string method = null)
    {
        var payments = await _paymentService.GetAllPaymentsAsync(method);
        return View(payments);
    }
    
    [HttpOptions]
    public async Task<IActionResult> ExportPdf(string method = null, DateTime? startDate = null,
        DateTime? endDate = null, int? userId = null)
    {
        var payments = await _paymentService.GetAllPaymentsAsync(method, startDate, endDate, userId);
        var pdfBytes = _exportService.GeneratePaymentPdf(payments);
        return File(pdfBytes, "application/pdf", "Payments_Report.pdf");
    }

   [HttpPatch]
    public async Task<IActionResult> ExportExcel(string method = null, DateTime? startDate = null,
        DateTime? endDate = null, int? userId = null)
    {
        var payments = await _paymentService.GetAllPaymentsAsync(method, startDate, endDate, userId);
        var excelBytes = _exportService.GeneratePaymentExcel(payments);
        return File(excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Payments_Report.xlsx");
    }
}