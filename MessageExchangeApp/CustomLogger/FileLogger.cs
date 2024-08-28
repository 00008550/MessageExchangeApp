namespace MessageExchangeApp.CustomLogger
{
    public class FileLogger : ILogger
    {
        private readonly string _logDirectory;
        private readonly object _lock = new object();

        public FileLogger(string logDirectory)
        {
            _logDirectory = logDirectory;
            // Ensure the directory exists
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}: {formatter(state, exception)}{Environment.NewLine}";

            // Generate the file name with the current date
            var filePath = Path.Combine(_logDirectory, $"app-log-{DateTime.UtcNow:yyyy-MM-dd}.txt");

            lock (_lock)
            {
                File.AppendAllText(filePath, message);
            }
        }
    }

    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _logDirectory;

        public FileLoggerProvider(string logDirectory)
        {
            _logDirectory = logDirectory;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_logDirectory);
        }

        public void Dispose() { }
    }

}
