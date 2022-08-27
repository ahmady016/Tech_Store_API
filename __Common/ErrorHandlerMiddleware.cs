using System.Net;

using DB.Common;

namespace Common;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<Entity> _logger;
    private readonly List<int> StatusCodes = new() { 401, 403, 404, 409, 422 };
    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<Entity> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpRequestException httpException)
        {
            string _errorMessage = httpException.Message;
            int _statusCode = (int)httpException.StatusCode;

            _logger.LogError(_errorMessage);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Contains(_statusCode) ? _statusCode : (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(_errorMessage);
        }
        catch (Exception error)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(error.Message);
        }
    }
}
