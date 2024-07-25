namespace LogParseTakeHome.Models
{
    public class LogEntry
    {
        public string Host { get; set; }
        public DateTime DateTime { get; set; }
        public string Operation { get; set; }
        public string Endpoint { get; set; }
        public int ReturnCode { get; set; }
        public int ReturnSize { get; set; }
    }
}
