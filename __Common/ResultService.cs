
namespace TechStoreApi.Common;

public interface IResultService
{
    IResult NotFound(string sender, string entity, Guid id);
    IResult NotFound(string sender, string message);
    IResult AlreadyExists(string sender, string entity, string propName);
    IResult AlreadyExists(string sender, string message);
    IResult Conflict(string sender, string message);
    IResult RemoveRestricted(string sender, string entity, Guid id, string childrenName);
    IResult BadRequest(string sender, string message);
    IResult ParameterlessSearch(string sender);
    IResult InvalidGuid(string sender, string entity, Guid id);
    IResult Created(string sender, string entity, Guid id);
    IResult Created(string sender, string message);
    IResult Updated(string sender, string entity, Guid id);
    IResult Removed(string sender, string entity, Guid id);
    IResult Removed(string sender, string message);
    IResult Succeeded(string message);
}

public class ResultService : IResultService
{
    private readonly ILogger<Object> _logger;
    private string _message;
    public ResultService(ILogger<Object> logger)
    {
        _logger = logger;
    }

    public IResult NotFound(string sender, string entity, Guid id)
    {
        _message = $"{sender} => {entity} Record with Id: [{id}] Not Found";
        _logger.LogError(_message);
        return Results.NotFound(new { Message = _message });
    }
    public IResult NotFound(string sender, string message)
    {
        _message = $"{sender} => {message}";
        _logger.LogError(_message);
        return Results.NotFound(new { Message = _message });
    }
    public IResult AlreadyExists(string sender, string entity, string propName)
    {
        _message = $"{sender} => {entity} [{propName}] already exists.";
        _logger.LogError(_message);
        return Results.Conflict(new { Message = _message });
    }
    public IResult AlreadyExists(string sender, string message)
    {
        _message = $"{sender} => {message}";
        _logger.LogError(_message);
        return Results.Conflict(new { Message = _message });
    }
    public IResult Conflict(string sender, string message)
    {
        _message = $"{sender} => {message}";
        _logger.LogError(_message);
        return Results.Conflict(new { Message = _message });
    }
    public IResult RemoveRestricted(string sender, string entity, Guid id, string childrenName)
    {
        _message = $"{sender} => {entity} Record with Id: {id} Can't Removed because it have some {childrenName}";
        _logger.LogError(_message);
        return Results.BadRequest(new { Message = _message });
    }
    public IResult BadRequest(string sender, string message)
    {
        _message = $"{sender} => {message}";
        _logger.LogError(_message);
        return Results.BadRequest(new { Message = _message });
    }
    public IResult InvalidGuid(string sender, string entity, Guid id)
    {
        _message = $"{sender} => {entity} Id: [{id}] Not a valid Id value";
        _logger.LogError(_message);
        return Results.BadRequest(new { Message = _message });
    }
    public IResult ParameterlessSearch(string sender)
    {
        _message = $"{sender} => Must supply at least one of the following: [where, select, orderBy]";
        _logger.LogError(_message);
        return Results.BadRequest(new { Message = _message });
    }
    public IResult Created(string sender, string entity, Guid id)
    {
        _message = $"{sender} => {entity} Record has been created successfully with Id: [{id}]";
        return Results.Json(new { Message = _message }, null, null, 201);
    }
    public IResult Created(string sender, string message)
    {
        _message = $"{sender} => {message}";
        return Results.Json(new { Message = _message }, null, null, 201);
    }
    public IResult Updated(string sender, string entity, Guid id)
    {
        _message = $"{sender} => {entity} Record with Id: [{id}] has been updated successfully.";
        return Results.Ok(new { Message = _message });
    }
    public IResult Removed(string sender, string entity, Guid id)
    {
        _message = $"{sender} => {entity} Record with Id: [{id}] has been removed successfully.";
        return Results.Ok(new { Message = _message });
    }
    public IResult Removed(string sender, string message)
    {
        _message = $"{sender} => {message}";
        return Results.Ok(new { Message = _message });
    }
    public IResult Succeeded(string message)
    {
        return Results.Ok(new { Message = message });
    }
}
