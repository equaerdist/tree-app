using System.Text.Json;

namespace tree_api.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, AppDbContext dbContext)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var eventId = DateTimeOffset.UtcNow.Ticks;
            var request = context.Request;

            // Чтение параметров запроса (query и body)
            var queryParams = request.Query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            string bodyParams = string.Empty;
            request.EnableBuffering();
            using (var reader = new StreamReader(request.Body, leaveOpen: true))
            {
                request.Body.Position = 0;
                bodyParams = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            // Логирование в журнал
            var journal = new ExceptionJournal
            {
                EventId = eventId,
                CreatedAt = DateTime.UtcNow,
                Text = JsonSerializer.Serialize(new
                {
                    Query = queryParams,
                    Body = bodyParams,
                    Exception = ex.ToString()
                })
            };

            dbContext.ExceptionJournals.Add(journal);
            await dbContext.SaveChangesAsync();

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            if (ex is SecureException secure)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "Secure",
                    id = eventId,
                    data = new { message = secure.Message }
                });
            }
            else
            {
                _logger.LogError(ex, "Unhandled exception");

                await context.Response.WriteAsJsonAsync(new
                {
                    type = "Exception",
                    id = eventId,
                    data = new { message = $"Internal server error ID = {eventId}" }
                });
            }
        }
    }
}

