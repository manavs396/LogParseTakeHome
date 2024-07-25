
# **Log File Parsing Assignment**

## **Description**

This project is a web application that provides quick analysis of web server logs after the launch of an important new product. The analysis includes:

- **Number of accesses to the web server per host, sorted with the host with the most entries at the top.**
- **Number of successful resource accesses by URI. Only “GET” accesses resulting in the HTTP reply code 200 (“OK”) are counted.**

## **Features**

- Parse web server log files to extract relevant information.
- Display the number of accesses to the web server per host.
- Display the number of successful resource accesses by URI.
- Filter log entries based on a user-defined time span.

## **Technologies Used**

- .NET Core (C#) for the backend
- ASP.NET Core Razor Pages for the frontend
- NUnit for unit testing

## **Getting Started**

### **Prerequisites**

- .NET SDK 6.0 or later
- Visual Studio 2022 or Visual Studio Code

### **Installation**

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/your-repo-name.git
   cd your-repo-name
   ```

2. Open the solution in Visual Studio or Visual Studio Code.

### **Running the Application**

1. Build the solution:

   ```bash
   dotnet build
   ```

2. Run the application:

   ```bash
   dotnet run
   ```

3. Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`.

### **Usage**

1. Upload your log file named `epa-http.txt` to the root directory of the project.
2. Use the web interface to specify the start and end date/time for filtering log entries.
3. Click the "Filter" button to see the results.

### **Unit Tests**

1. To run the unit tests, use the following command:

   ```bash
   dotnet test
   ```

## **Project Structure**

- **Models**: Contains the `LogEntry` class representing a log entry.
- **Services**: Contains the `LogParser` class responsible for parsing the log file and performing the analysis.
- **Pages**: Contains the Razor Pages for the web interface.
- **Tests**: Contains the unit tests for the `LogParser` class.

## **Code Overview**

### **LogParser.cs**

This file contains methods for parsing the log file and performing the required analysis.

- `ParseLogFile(string filePath)`: Reads and parses the log file, returning a list of `LogEntry` objects.
- `ProcessLogEntry(string logEntry)`: Processes a single log entry and adds it to the list of log entries.
- `SplitLogEntry(string logEntry)`: Splits a log entry string into its components.
- `ParseDateTime(string dateTime)`: Parses the date and time string into a `DateTime` object.
- `CountAccessesPerHost(List<LogEntry> logEntries)`: Counts the number of accesses per host.
- `CountSuccessfulAccessesByUri(List<LogEntry> logEntries)`: Counts the number of successful accesses by URI.

### **Index.cshtml.cs**

This file contains the logic for the web interface.

- `StartDateTime` and `EndDateTime`: Bind properties for the start and end date/time filters.
- `HostAccessCounts` and `SuccessfulAccessCounts`: Properties to hold the analysis results.
- `OnGet()`: Handles the GET request to load the page and perform the analysis based on the specified date/time filters.

### **Unit Tests**

The unit tests are written using NUnit and cover the following scenarios:

- `ParseLogFile_ParsesValidLogEntries`: Tests if the log entries are parsed correctly.
- `CountAccessesPerHost_ReturnsCorrectCounts`: Tests if the number of accesses per host is counted correctly.
- `CountSuccessfulAccessesByUri_ReturnsCorrectCounts`: Tests if the number of successful accesses by URI is counted correctly.

## **License**

This project is licensed under the MIT License - see the LICENSE file for details.

## **Acknowledgements**

- Thanks to the developers of .NET Core, ASP.NET Core, and NUnit for providing the tools and frameworks used in this project.
