using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;
using VetClinicManager.DTOs.Visits;

namespace VetClinicManager.Models
{
    public class VisitPdfReport : IDocument
    {
        private readonly VisitReportDto _visitData;

        public VisitPdfReport(VisitReportDto visitData)
        {
            _visitData = visitData;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(30);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text("Raport wygenerowany przez system Szwagromed");
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Raport z Wizyty Weterynaryjnej").SemiBold().FontSize(24).FontColor(Colors.Blue.Darken2);
                    column.Item().Text($"Wizyta: {_visitData.Title}").FontSize(16);
                    column.Item().Text($"Pacjent: {_visitData.Animal.Name} ({_visitData.Animal.Species})");
                    column.Item().Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(25);
                column.Item().Element(ComposeVisitDetails);
                if (_visitData.Updates.Any())
                {
                    column.Item().Text("Historia Wizyty").SemiBold().FontSize(18);
                    column.Item().Element(ComposeUpdatesTable);
                }
            });
        }

        private void ComposeVisitDetails(IContainer container)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(column =>
            {
                column.Item().Text("Podstawowe Informacje").Bold().FontSize(14);
                column.Item().Grid(grid =>
                {
                    grid.Columns(12);
                    grid.Item(6).Text($"Status: {GetEnumDisplayName(_visitData.Status)}");
                    if (_visitData.Priority.HasValue)
                    {
                        grid.Item(6).Text($"Priorytet: {GetEnumDisplayName(_visitData.Priority.Value)}");
                    }
                    grid.Item(6).Text($"Data utworzenia: {_visitData.CreatedDate:dd.MM.yyyy}");
            
                    var vetName = _visitData.AssignedVet != null ? $"{_visitData.AssignedVet.FirstName} {_visitData.AssignedVet.LastName}" : "Nieprzypisany";
                    grid.Item(6).Text($"Lekarz prowadzący: {vetName}");

                    if (_visitData.Owner != null)
                    {
                        var ownerName = $"{_visitData.Owner.FirstName} {_visitData.Owner.LastName}";
                        grid.Item(12).PaddingTop(5).Text($"Właściciel: {ownerName}");
                    }
                });
            });
        }

        private void ComposeUpdatesTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Text("Data Aktualizacji");
                    header.Cell().Background(Colors.Grey.Lighten2).Text("Weterynarz");
                    header.Cell().Background(Colors.Grey.Lighten2).Text("Notatki");
                    header.Cell().Background(Colors.Grey.Lighten2).Text("Podane Leki");
                });

                foreach (var update in _visitData.Updates.OrderBy(u => u.UpdateDate))
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(update.UpdateDate.ToString("g"));
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(update.UpdatedByVetName);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(update.Notes);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Column(col =>
                    {
                        if (update.Medications.Any())
                        {
                            foreach (var med in update.Medications)
                            {
                                col.Item().Text($"{med.Name} (do: {med.EndDate:dd.MM.yyyy})");
                            }
                        }
                    });
                }
            });
        }

        private string GetEnumDisplayName(Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?
                            .GetName() ?? enumValue.ToString();
        }
    }
}