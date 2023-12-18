namespace StoreAPI.Middleware;

public class FileLogger : ILogger
{
    private readonly string _logFilePath;

    public FileLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var logRecord = formatter(state, exception);
        var logMessage = $"[{DateTimeOffset.UtcNow:yyyy-MM-dd HH}] [{logLevel}] {logRecord}{Environment.NewLine}";

        using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        using var sw = new StreamWriter(fileStream);
        sw.Write(logMessage);
    }
}
