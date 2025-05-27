using System.Net;
using tree_api.Domain.Exceptions;
using tree_api.Domain.Services.ExceptionHandler;

namespace tree_api.API.Middlewares;

internal sealed class ExceptionLoggingMiddleware
{
    internal const string AppJson = "application/json";
    private readonly RequestDelegate _next;

    public ExceptionLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IExceptionHandler handler)
    {
        try
        {
            await _next(context);
        }
        catch (SecureException ex)
        {
            var id = await handler.HandleAsync(
                ex,
                await ReadRequestBodyAsync(context.Request),
                context.TraceIdentifier,
                context.Request.QueryString.ToString());

            var result = new
            {
                type = "Secure",
                id,
                data = new { message = ex.Message }
            };
            await WriteResultToContext(context, HttpStatusCode.BadRequest, result);
        }
        catch (Exception ex)
        {
            var id = await handler.HandleAsync(
                ex,
                await ReadRequestBodyAsync(context.Request),
                context.TraceIdentifier,
                context.Request.QueryString.ToString());

            var result = new
            {
                type = "Exception",
                id,
                data = new { message = $"Internal server error ID = {id}" }
            };
            await WriteResultToContext(context, HttpStatusCode.InternalServerError, result);
        }
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        try
        {
            request.EnableBuffering();
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
        catch
        {
            return null;
        }
    }

    private static async Task WriteResultToContext(HttpContext context, HttpStatusCode code, object result)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = AppJson;

        await context.Response.WriteAsJsonAsync(result);
    }
}
