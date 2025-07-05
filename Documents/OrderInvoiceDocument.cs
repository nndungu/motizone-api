using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MotiZone.Models;

namespace MotiZone.PdfDocuments;

public class OrderInvoiceDocument : IDocument
{
    private readonly Order _order;

    public OrderInvoiceDocument(Order order)
    {
        _order = order;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header()
                .Text($"Order Invoice #{_order.Id}")
                .FontSize(20)
                .Bold()
                .AlignCenter();

            page.Content()
                .Column(col =>
                {
                    col.Item().Text($"Customer: {_order.User?.UserName}");
                    col.Item().Text($"Date: {_order.OrderDate:yyyy-MM-dd}");

                    col.Item().LineHorizontal(1);

                    foreach (var item in _order.Items)
                    {
                        col.Item().Text($"{item.Part.Name} - {item.Quantity} x {item.Part.Price:C}");
                    }

                    col.Item().LineHorizontal(1);
                    col.Item().Text($"Total: {_order.Items.Sum(x => x.Quantity * x.Part.Price):C}")
                        .FontSize(14).Bold();
                });

            page.Footer()
                .AlignCenter()
                .Text("Thank you for your order!");
        });
    }
}