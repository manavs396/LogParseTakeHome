using LogParseTakeHome.Models;
using LogParseTakeHome.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace LogParseTakeHome
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParseLogFile_ParsesValidLogEntries()
        {
            // Arrange
            var logContent = @"
wpbfl2-45.gate.net [29:23:55:29] ""GET /docs/Access HTTP/1.0"" 302 -
140.112.68.165 [29:23:55:33] ""GET /logos/us-flag.gif HTTP/1.0"" 200 2788
wpbfl2-45.gate.net [29:23:55:46] ""GET /information.html HTTP/1.0"" 200 617";

            var logFilePath = Path.Combine(Path.GetTempPath(), "testlog.txt");
            File.WriteAllText(logFilePath, logContent);

            // Act
            var logEntries = LogParser.ParseLogFile(logFilePath);

            // Debug output to verify the contents
            Console.WriteLine("Log Entries Count: " + logEntries.Count);
            foreach (var entry in logEntries)
            {
                Console.WriteLine($"Host: {entry.Host}, DateTime: {entry.DateTime}, Request: {entry.Operation} {entry.Endpoint}, ReturnCode: {entry.ReturnCode}, ReturnSize: {entry.ReturnSize}");
            }

            // Assert
            Assert.AreEqual(3, logEntries.Count);
            Assert.AreEqual("wpbfl2-45.gate.net", logEntries[0].Host);
            Assert.AreEqual(new DateTime(2024, 8, 29, 23, 55, 29), logEntries[0].DateTime); // UTC conversion
            Assert.AreEqual("GET", logEntries[0].Operation);
            Assert.AreEqual("/docs/Access", logEntries[0].Endpoint);
            Assert.AreEqual(302, logEntries[0].ReturnCode);
            Assert.AreEqual(0, logEntries[0].ReturnSize); // "-" means 0
        }

        [Test]
        public void CountAccessesPerHost_ReturnsCorrectCounts()
        {
            // Arrange
            var logEntries = new List<LogEntry>
            {
                new LogEntry { Host = "host1", DateTime = DateTime.Now, Operation = "GET", Endpoint = "/index.html", ReturnCode = 200, ReturnSize = 100 },
                new LogEntry { Host = "host1", DateTime = DateTime.Now, Operation = "GET", Endpoint = "/about.html", ReturnCode = 200, ReturnSize = 100 },
                new LogEntry { Host = "host2", DateTime = DateTime.Now, Operation = "GET", Endpoint = "/index.html", ReturnCode = 200, ReturnSize = 100 }
            };

            // Act
            var result = LogParser.CountAccessesPerHost(logEntries);

            // Assert
            Assert.AreEqual(2, result["host1"]);
            Assert.AreEqual(1, result["host2"]);
        }

        [Test]
        public void CountSuccessfulAccessesByUri_ReturnsCorrectCounts()
        {
            // Arrange
            var logEntries = new List<LogEntry>
            {
                new LogEntry { Host = "host1", DateTime = DateTime.Now, Operation = "GET", Endpoint = "/index.html", ReturnCode = 200, ReturnSize = 100 },
                new LogEntry { Host = "host1", DateTime = DateTime.Now, Operation = "GET", Endpoint = "/about.html", ReturnCode = 404, ReturnSize = 100 },
                new LogEntry { Host = "host2", DateTime = DateTime.Now, Operation = "GET", Endpoint = "/index.html", ReturnCode = 200, ReturnSize = 100 }
            };

            // Act
            var result = LogParser.CountSuccessfulAccessesByUri(logEntries);

            // Assert
            Assert.AreEqual(2, result["/index.html"]);
            Assert.False(result.ContainsKey("/about.html"));
        }

        [Test]
        public void ParseLogFile_HandlesEmptyLogContent()
        {
            // Arrange
            var logContent = "";
            var logFilePath = Path.Combine(Path.GetTempPath(), "testlog_empty.txt");
            File.WriteAllText(logFilePath, logContent);

            // Act
            var logEntries = LogParser.ParseLogFile(logFilePath);

            // Assert
            Assert.AreEqual(0, logEntries.Count);
        }

        [Test]
        public void ParseLogFile_HandlesLogEntriesWithMoreThanTwoQuotes()
        {
            // Arrange
            var logContent = @"
bastion.fdic.gov [30:09:36:25] ""GET /enviro/gif/blueball.gifalt="" HTTP/1.0"" 404 -";

            var logFilePath = Path.Combine(Path.GetTempPath(), "testlog_more_than_two_quotes.txt");
            File.WriteAllText(logFilePath, logContent);

            // Act
            var logEntries = LogParser.ParseLogFile(logFilePath);

            // Debug output to verify the contents
            Console.WriteLine("Log Entries Count: " + logEntries.Count);
            foreach (var entry in logEntries)
            {
                Console.WriteLine($"Host: {entry.Host}, DateTime: {entry.DateTime}, Request: {entry.Operation} {entry.Endpoint}, ReturnCode: {entry.ReturnCode}, ReturnSize: {entry.ReturnSize}");
            }

            // Assert
            Assert.AreEqual(1, logEntries.Count);
            Assert.AreEqual("bastion.fdic.gov", logEntries[0].Host);
            Assert.AreEqual(new DateTime(2024, 8, 30, 9, 36, 25), logEntries[0].DateTime); // UTC conversion
            Assert.AreEqual("GET", logEntries[0].Operation);
            Assert.AreEqual("/enviro/gif/blueball.gifalt=", logEntries[0].Endpoint);
            Assert.AreEqual(404, logEntries[0].ReturnCode);
            Assert.AreEqual(0, logEntries[0].ReturnSize); // "-" means 0
        }
    }
}
