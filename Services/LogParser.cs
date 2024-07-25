using LogParseTakeHome.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogParseTakeHome.Services
{
    public class LogParser
    {
        private static List<LogEntry> logEntries = new List<LogEntry>();

        public static List<LogEntry> ParseLogFile(string filePath)
        {
            logEntries.Clear(); // Clear previous entries
            var lines = File.ReadAllLines(filePath);
            var multilineLog = "";

            foreach (var line in lines)
            {
                if (line.StartsWith(" ") || line.StartsWith("\t"))
                {
                    multilineLog += line.Trim();
                }
                else
                {
                    if (!string.IsNullOrEmpty(multilineLog))
                    {
                        ProcessLogEntry(multilineLog);
                        multilineLog = "";
                    }
                    multilineLog = line;
                }
            }

            if (!string.IsNullOrEmpty(multilineLog))
            {
                ProcessLogEntry(multilineLog);
            }

            return logEntries;
        }

        private static void ProcessLogEntry(string logEntry)
        {
            var parts = SplitLogEntry(logEntry);

            if (parts.Length == 5)
            {
                string operation;
                string endpoint;
                var requestParts = parts[2].Split(' ');
                if (requestParts.Length >= 2)
                {
                    operation = requestParts[0];
                    endpoint = requestParts[1];
                }
                else
                {
                    operation = "";
                    endpoint = requestParts[0];
                }
                var log = new LogEntry
                {
                    Host = parts[0],
                    DateTime = ParseDateTime(parts[1]),
                    Operation = operation,
                    Endpoint = endpoint,
                    ReturnCode = int.Parse(parts[3]),
                    ReturnSize = parts[4] == "-" ? 0 : int.Parse(parts[4])
                };

                logEntries.Add(log);
            }
            else
            {
                Console.WriteLine($"Invalid log entry format: {parts[0] + "#" + parts[1] + "#" + parts[2] + "#" + parts[3]}");
            }
        }

        private static string[] SplitLogEntry(string logEntry)
        {
            var parts = new List<string>();
            var currentPart = string.Empty;
            var inQuotes = false;

            if (logEntry.Count(c => c == '"') == 2)
            {
                for (int i = 0; i < logEntry.Length; i++)
                {
                    var currentChar = logEntry[i];

                    if (currentChar == '"')
                    {
                        inQuotes = !inQuotes;
                    }
                    else if (currentChar == ' ' && !inQuotes)
                    {
                        if (!string.IsNullOrWhiteSpace(currentPart))
                        {
                            parts.Add(currentPart.Trim());
                            currentPart = string.Empty;
                        }
                    }
                    else
                    {
                        currentPart += currentChar;
                    }
                }

                if (!string.IsNullOrWhiteSpace(currentPart))
                {
                    parts.Add(currentPart.Trim());
                }
                return parts.ToArray();
            }
            else
            {
                int firstQuoteIndex = logEntry.IndexOf('"');
                int lastQuoteIndex = logEntry.LastIndexOf('"');

                string betweenQuotes = logEntry.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1);


                string beforeQuotes = logEntry[..firstQuoteIndex].Trim();
                string afterQuotes = logEntry[(lastQuoteIndex + 1)..].Trim();

                string[] beforeSplit = beforeQuotes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] afterSplit = afterQuotes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string[] result = beforeSplit.Concat(new[] { betweenQuotes }).Concat(afterSplit).ToArray();
                return result;
            }
        }

        private static DateTime ParseDateTime(string dateTime)
        {
            dateTime = dateTime.Trim('[', ']');
            var parts = dateTime.Split(':');
            var day = int.Parse(parts[0]);
            var hour = int.Parse(parts[1]);
            var minute = int.Parse(parts[2]);
            var second = int.Parse(parts[3]);

            var date = new DateTime(2024, 8, day, hour, minute, second, DateTimeKind.Utc);
            return date;
        }

        public static Dictionary<string, int> CountAccessesPerHost(List<LogEntry> logEntries)
        {
            return logEntries.GroupBy(entry => entry.Host)
                             .OrderByDescending(group => group.Count())
                             .ToDictionary(group => group.Key, group => group.Count());
        }

        public static Dictionary<string, int> CountSuccessfulAccessesByUri(List<LogEntry> logEntries)
        {
            return logEntries.Where(entry => entry.ReturnCode == 200)
                             .GroupBy(entry => entry.Endpoint)
                             .ToDictionary(group => group.Key, group => group.Count());
        }
    }
}
