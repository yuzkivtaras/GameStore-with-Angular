namespace StoreAPI.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            string ipAddress = httpContext.Connection.RemoteIpAddress.ToString();

            using (var fileStream = new FileStream("requests.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using var sw = new StreamWriter(fileStream);
                sw.WriteLine($"Request from IP: {ipAddress}");
            }

            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LoggingMiddleware)}: Error level logging with {ex.Message}");
            throw;
        }
    }
}