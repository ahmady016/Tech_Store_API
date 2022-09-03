using System.Net;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Models.Commands;
public class AddManyModelsCommand : IRequest<IResult>
{
    public List<AddModelCommand> NewModels { get; set; }
}

public class AddManyModelsCommandHandler : IRequestHandler<AddManyModelsCommand, IResult>
{
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public AddManyModelsCommandHandler(
        IDBService dbService,
        ICrudService crudService,
        ILogger<Product> logger
    )
    {
        _dbService = dbService;
        _crudService = crudService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        AddManyModelsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get all inputs titles
        var titles = command.NewModels.Select(e => e.Title).ToList();
        // check if any title are existed in db then reject the command and return error
        var existedModels = _dbService.GetList<Model>(p => titles.Contains(p.Title));
        if (existedModels.Count > 0)
        {
            _errorMessage = $"Some of NewModels Titles already existed.";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // do the normal Add action
        var createdModels = _crudService.AddMany<Model, ModelDto, AddModelCommand>(command.NewModels);
        return await Task.FromResult(Results.Ok(createdModels));
    }

}
