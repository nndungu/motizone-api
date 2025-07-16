using MotiZone.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace MotiZone.Services;

public class PdfInvoiceService
{
    public byte[] GenerateInvoice(Order order)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Header().Text($"Invoice #{order.Id}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.ConstantColumn(100);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Item");
                        header.Cell().Text("Price");
                    });

                    foreach (var item in order.Items)
                    {
                        var name = item.Vehicle?.Name ?? item.Part?.Name ?? item.Customization?.Name;
                        var price = item.Vehicle?.Price ?? item.Part?.Price ?? item.Customization?.Price;

                        table.Cell().Text(name);
                        table.Cell().Text($"KES {price}");
                    }

                    table.Cell().Text("Total").Bold();
                    table.Cell().Text($"KES {order.TotalAmount}").Bold();
                });

                page.Footer().AlignCenter().Text("Thank you for shopping with MotiZone!");
            });
        }).GeneratePdf();
    }
}
