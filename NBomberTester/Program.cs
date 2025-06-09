using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;

namespace VetClinicManager.PerformanceTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://localhost:5285/Animals";
            
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            
            using var httpClient = new HttpClient(handler);
            
            var scenario = Scenario.Create("get_visits_page_scenario", async context =>
            {
                var response = await httpClient.GetAsync($"{baseUrl}/Visits");

                return response.IsSuccessStatusCode 
                    ? Response.Ok() 
                    : Response.Fail();
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(1))
            .WithLoadSimulations(
                Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(2))
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("performance_report_final")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
                .Run();
                
            Console.WriteLine("Test zakończony");
            Console.ReadKey();
        }
    }
}