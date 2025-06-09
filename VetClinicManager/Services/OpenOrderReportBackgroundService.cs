using VetClinicManager.Services.Reports;

namespace VetClinicManager.Services;

public class OpenOrderReportBackgroundService : BackgroundService
{
    private readonly ILogger<OpenOrderReportBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public OpenOrderReportBackgroundService(ILogger<OpenOrderReportBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Usługa raportowania w tle została uruchomiona.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var delay = TimeSpan.FromMinutes(1);
                await Task.Delay(delay, stoppingToken);

                _logger.LogInformation("Rozpoczynam generowanie raportu dziennego...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var visitService = scope.ServiceProvider.GetRequiredService<IVisitService>();
                    var emailSender = scope.ServiceProvider.GetRequiredService<EmailSender>();
                    var reportGenerator = scope.ServiceProvider.GetRequiredService<PdfReportGenerator>();

                    var openVisits = await visitService.GetOpenVisitsForReportAsync();

                    if (!openVisits.Any())
                    {
                        _logger.LogInformation("Brak otwartych wizyt do zaraportowania. Pomijam wysyłkę.");
                        continue;
                    }

                    var pdfData = reportGenerator.GenerateOpenVisitsReport(openVisits);
                    var reportName = $"raport-otwarte-wizyty-{DateTime.Now:yyyy-MM-dd}.pdf";

                    var adminEmail = "lesoro7159@adrewire.com";
                    var subject = $"Dzienny raport otwartych wizyt - {DateTime.Now:dd.MM.yyyy}";
                    var body = "W załączniku znajduje się automatycznie wygenerowany raport otwartych wizyt.";

                    await emailSender.SendEmailWithAttachmentAsync(adminEmail, subject, body, pdfData, reportName);

                    _logger.LogInformation($"Raport został pomyślnie wygenerowany i wysłany na adres {adminEmail}");
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Usługa raportowania została zatrzymana.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas generowania lub wysyłania raportu.");
            }
        }
    }
}