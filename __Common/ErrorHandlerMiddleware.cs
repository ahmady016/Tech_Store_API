using System.Text;
using System.Net;
using System.Text.Json;

namespace TechStoreApi.Common;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public override string ToString() => JsonSerializer.Serialize(this);
}

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<Object> _logger;
    private readonly List<int> StatusCodes = new() { 401, 403, 404, 409, 422 };
    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<Object> logger)
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
            await context.Response.WriteAsync(
                new ErrorResponse()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = _errorMessage
                    }
                    .ToString()
            );
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(
                new ErrorResponse()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = ex.GetFullErrorMessage()
                    }
                    .ToString()
            );
        }
    }
}
