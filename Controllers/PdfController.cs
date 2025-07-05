using Microsoft.AspNetCore.Mvc;
using MotiZone.Services;
using MotiZone.PdfDocuments;
using QuestPDF.Fluent; // You will create this for QuestPDF documents

namespace MotiZone.Controllers;

public class PdfController : Controller
{
    private readonly OrderService _orderService;

    public PdfController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Invoice(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();

        var document = new OrderInvoiceDocument(order); // ← a QuestPDF document class
        var pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"Invoice_{order.Id}.pdf");
    }
}