using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MotiZone.Models;

namespace MotiZone.PdfDocuments;

public class RentalInvoiceDocument : IDocument
{
    private readonly Rental _rental;

    public RentalInvoiceDocument(Rental rental)
    {
        _rental = rental;
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
                .Text($"Rental Invoice #{_rental.Id}")
                .FontSize(20)
                .Bold()
                .AlignCenter();

            page.Content()
                .Column(col =>
                {
                    col.Item().Text($"Customer: {_rental.User?.UserName ?? "N/A"}");
                    col.Item().Text($"Vehicle: {_rental.Vehicle?.Make} {_rental.Vehicle?.Model}");
                    col.Item().Text($"Rental Period: {_rental.StartDate:yyyy-MM-dd} to {_rental.EndDate:yyyy-MM-dd}");
                    col.Item().Text($"Total Days: {(_rental.EndDate - _rental.StartDate).Days}");
                    col.Item().Text($"Daily Rate: {_rental.DailyRate:C}");
                    col.Item().LineHorizontal(1);

                    var total = (_rental.EndDate - _rental.StartDate).Days * _rental.DailyRate;
                    col.Item().Text($"Total: {total:C}")
                        .FontSize(14)
                        .Bold();
                });

            page.Footer()
                .AlignCenter()
                .Text("Thank you for renting with MotiZone!");
        });
    }
}