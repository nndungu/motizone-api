using Microsoft.AspNetCore.Mvc;
using MotiZone.Services;
using MotiZone.PdfDocuments;
using QuestPDF.Fluent;

namespace MotiZone.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly OrderService _orderService;

        public InvoiceController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("/invoice/pdf/{id}")]
        public async Task<IActionResult> Pdf(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var doc = new OrderInvoiceDocument(order);
            var stream = doc.GeneratePdf();

            return File(stream, "application/pdf", $"Invoice_{id}.pdf");
        }
    }
}