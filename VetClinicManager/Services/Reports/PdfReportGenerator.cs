using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;

namespace VetClinicManager.Services.Reports;

public class PdfReportGenerator
{
    public byte[] GenerateOpenVisitsReport(IEnumerable<Visit> openVisits)
    {
        QuestPDF.Settings.License = LicenseType.Community; 
        
        var document = new OpenVisitsDocument(openVisits);
        return document.GeneratePdf();
    }
}

public class OpenVisitsDocument : IDocument
{
    private readonly IEnumerable<Visit> _visits;

    public OpenVisitsDocument(IEnumerable<Visit> visits)
    {
        _visits = visits;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Raport Otwartych Wizyt").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                column.Item().Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}");
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Item().Element(ComposeTable);

            if (!_visits.Any())
            {
                column.Item().PaddingTop(20).Text("Brak otwartych wizyt na moment generowania raportu.").SemiBold();
            }
        });
    }

    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(4);
                columns.RelativeColumn(4);
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
            });

            table.Header(header =>
            {
                header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Tytuł");
                header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Zwierzę (Właściciel)");
                header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Lekarz");
                header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Data Wizyty");
                header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Status");
            });

            foreach (var visit in _visits)
            {
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                     .Text(visit.Title ?? "Brak tytułu");
                     
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                     .Text($"{visit.Animal?.Name ?? "B/D"} ({visit.Animal?.User?.FirstName ?? ""} {visit.Animal?.User?.LastName ?? ""})");
                     
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                     .Text($"{visit.AssignedVet?.FirstName ?? ""} {visit.AssignedVet?.LastName ?? "Nieprzypisany"}");
                     
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                     .Text($"{visit.CreatedDate:dd.MM.yyyy HH:mm}");
                     
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                     .Text(TranslateStatus(visit.Status));
            }
        });
    }
    
    private string TranslateStatus(VisitStatus status)
    {
        return status switch
        {
            VisitStatus.Scheduled => "Zaplanowana",
            VisitStatus.InProgress => "W trakcie",
            VisitStatus.Completed => "Zakończona",
            VisitStatus.Cancelled => "Anulowana",
            _ => status.ToString()
        };
    }
}