using tree_api.Database;
using tree_api.Database.Models;

namespace tree_api.Domain.Services.ExceptionHandler;

internal sealed class ExceptionHandler : IExceptionHandler
{
    private readonly DbCtx _dbContext;
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(DbCtx dbContext, ILogger<ExceptionHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<long> HandleAsync(
        Exception exception,
        string? request = null,
        string? requestId = null,
        string? queryParams = null)
    {
        try
        {
            var logEntry = new LogEntry
            {
                EventId = requestId,
                CreatedAt = DateTimeOffset.UtcNow,
                StackTrace = exception.ToString(),
                QueryParameters = queryParams,
                BodyParameters = request
            };

            _dbContext.LogJournal.Add(logEntry);
            await _dbContext.SaveChangesAsync();

            return logEntry.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logging request exception...");
            return default;
        }
    }
}

