using System.Net;
using MediatR;

using DB;
using Entities;
using Dtos;

namespace Models.Commands;

public class UpdateManyModelsCommand : IRequest<IResult>
{
    public List<UpdateModelCommand> ModifiedModels { get; set; }
}

public class UpdateManyModelsCommandHandler : IRequestHandler<UpdateManyModelsCommand, IResult> {
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateManyModelsCommandHandler(
        IDBService dbService,
        ICrudService crudService,
        ILogger<Brand> logger
    )
    {
        _dbService = dbService;
        _crudService = crudService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateManyModelsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get oldModels from db
        var oldModels = _crudService.GetByIds<Model>(command.ModifiedModels.Select(p => p.Id).ToList());

        // get existedModelsTitles and modifiedModelsTitles
        var existedModelsTitles = oldModels.Select(p => p.Title);
        var modifiedModelsTitles = command.ModifiedModels.Select(p => p.ModifiedEntity.Title);
        // get changedModelsTitles
        var changedModelsTitles = modifiedModelsTitles
            .Where(title => !existedModelsTitles.Contains(title))
            .ToList();
        // if any titles changed check if already existed in db and then reject all inputs
        if (changedModelsTitles.Count > 0)
        {
            var modelsWithSameTitle = _dbService.GetList<Model>(p => changedModelsTitles.Contains(p.Title));
            if (modelsWithSameTitle.Count > 0)
            {
                _errorMessage = $"Models List was rejected, Some Titles are already existed.";
                _logger.LogError(_errorMessage);
                throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
            }
        }

        // do the normal update many items action
        var updatedModels = _crudService.UpdateMany<Model, ModelDto, UpdateModelCommand, AddModelCommand>(command.ModifiedModels, oldModels);
        return await Task.FromResult(Results.Ok(updatedModels));
    }

}
