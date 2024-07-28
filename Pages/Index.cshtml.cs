using Microsoft.AspNetCore.Mvc.RazorPages;
using LogParseTakeHome.Services;
using LogParseTakeHome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public DateTime? StartDateTime { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDateTime { get; set; }

    public Dictionary<string, int>? HostAccessCounts { get; set; }
    public Dictionary<string, int>? SuccessfulAccessCounts { get; set; }

    public void OnGet()
    {
        // Ensure the year and month are fixed to August 2024
        if (StartDateTime.HasValue)
        {
            StartDateTime = new DateTime(2024, 8, StartDateTime.Value.Day, StartDateTime.Value.Hour, StartDateTime.Value.Minute, StartDateTime.Value.Second);
        }

        if (EndDateTime.HasValue)
        {
            EndDateTime = new DateTime(2024, 8, EndDateTime.Value.Day, EndDateTime.Value.Hour, EndDateTime.Value.Minute, EndDateTime.Value.Second);
        }

        var logFilePath = Path.Combine(Environment.CurrentDirectory, "epa-http.txt");
        var logEntries = LogParser.ParseLogFile(logFilePath);

        if (StartDateTime.HasValue && EndDateTime.HasValue)
        {
            logEntries = logEntries.Where(entry => entry.DateTime >= StartDateTime.Value && entry.DateTime <= EndDateTime.Value).ToList();
        }

        HostAccessCounts = LogParser.CountAccessesPerHost(logEntries);
        SuccessfulAccessCounts = LogParser.CountSuccessfulAccessesByUri(logEntries);
        WriteResultsToFile();
    }

    private void WriteResultsToFile()
    {
        var filePath = "log_analysis_results.txt";
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Number of accesses to webserver per host:");
            foreach (var entry in HostAccessCounts)
            {
                writer.WriteLine($"{entry.Key} {entry.Value}");
            }

            writer.WriteLine();
            writer.WriteLine("Number of successful resource accesses by URI:");
            foreach (var entry in SuccessfulAccessCounts)
            {
                writer.WriteLine($"{entry.Key} {entry.Value}");
            }
        }
    }
}
