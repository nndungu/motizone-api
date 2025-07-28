using Microsoft.AspNetCore.Mvc;
using MotiZone.Services;
using MotiZone.PdfDocuments;
using QuestPDF.Fluent;

namespace MotiZone.Controllers;
[Route("invoice")]
public class InvoiceController : Controller
{
    private readonly OrderService _orderService;

    public InvoiceController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        var document = new OrderInvoiceDocument(order);
        var pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"Invoice_{order.Id}.pdf");
    }
}