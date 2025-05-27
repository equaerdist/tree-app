namespace tree_api.Domain.Services.ExceptionHandler;

internal interface IExceptionHandler
{
    Task<long> HandleAsync(
        Exception exception, string? request = null, string? requestId = null, string? queryParams = null);
}
