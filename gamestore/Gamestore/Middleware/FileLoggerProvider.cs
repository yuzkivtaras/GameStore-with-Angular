namespace StoreAPI.Middleware;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _logFilePath;

    public FileLoggerProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _logFilePath = _configuration.GetValue<string>("Path");
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_logFilePath);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
