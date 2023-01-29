using Microsoft.AspNetCore.Mvc;

namespace TechStoreApi.Common;

[ApiController]
[Route("api/[controller]/[action]")]
public class EnumsController : ControllerBase
{
    private readonly IResultService _resultService;
    public EnumsController(IResultService resultService) => _resultService = resultService;
    private const string _enumsNamespace = "TechStoreApi.DB.Common";
    private dynamic _GetEnumInfo(object enumValue)
    {
        return new { Id = (byte)enumValue, Name = enumValue.ToString().Replace('_', ' ') };
    }
    private IEnumerable<Type> _GetAppTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(t => t.GetTypes());
    }

    [HttpGet]
    public IResult List()
    {
        var enumsList = _GetAppTypes()
            .Where(type => type.IsEnum && type.Namespace == _enumsNamespace)
            .Select(type =>
                {
                    var enumValues = Enum.GetValues(type).Cast<byte>();
                    var range = $"({enumValues.Min()}, {enumValues.Max()})";
                    return  new { Name = type.Name, Range = range };
                })
            .ToList();
        return Results.Ok(enumsList);
    }

    [HttpGet("{enumNames}")]
    public IResult FindMany(string enumNames)
    {
        var enumNamesList = enumNames.SplitAndRemoveEmpty(',');
        var enumsList = _GetAppTypes()
            .Where(type => type.Namespace == _enumsNamespace && type.IsEnum && enumNamesList.Contains(type.Name))
            .Select(type =>
                {
                    var values = new List<dynamic>();
                    foreach (var value in Enum.GetValues(type))
                        values.Add(_GetEnumInfo(value));
                    return  new { Name = type.Name, Values = values };
                })
            .ToList();
        return Results.Ok(enumsList);
    }

    [HttpGet("{enumName}")]
    public IResult Find(string enumName)
    {
        var enumType = Type.GetType($"{_enumsNamespace}.{enumName}");
        if(enumType is null)
            return _resultService.NotFound(nameof(EnumsController), "enum not found!!!");

        var result = new List<dynamic>();
        foreach (var value in Enum.GetValues(enumType))
            result.Add(_GetEnumInfo(value));

        return Results.Ok(result);
    }
}
