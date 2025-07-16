using ClosedXML.Excel;
using MotiZone.Models;
using QuestPDF.Fluent;

namespace MotiZone.Services;

public class ExportService
{
    public byte[] GeneratePaymentPdf(List<Payment> payments)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text("Payment Report").Bold().FontSize(16);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("User ID").Bold();
                        header.Cell().Text("Amount").Bold();
                        header.Cell().Text("Method").Bold();
                        header.Cell().Text("Date").Bold();
                    });

                    foreach (var p in payments)
                    {
                        table.Cell().Text(p.UserId.ToString());
                        table.Cell().Text($"KES {p.Amount}");
                        table.Cell().Text(p.PaymentMethod);
                        table.Cell().Text(p.PaidAt.ToShortDateString());
                    }
                });
            });
        }).GeneratePdf();
    }

    public byte[] GeneratePaymentExcel(List<Payment> payments)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Payments");

        sheet.Cell(1, 1).Value = "User ID";
        sheet.Cell(1, 2).Value = "Amount";
        sheet.Cell(1, 3).Value = "Method";
        sheet.Cell(1, 4).Value = "Date";
        
        sheet.Row(1).Style.Font.Bold = true;
        sheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < payments.Count; i++)
        {
            var p = payments[i];
            sheet.Cell(i + 2, 1).Value = p.UserId;
            sheet.Cell(i + 2, 2).Value = p.Amount;
            sheet.Cell(i + 2, 3).Value = p.PaymentMethod;
            sheet.Cell(i + 2, 4).Value = p.PaidAt;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
