using MediatR;

using DB;
using Entities;
using Dtos;
using Auth;

namespace Models.Commands;

public class UpdateManyModelsCommand : IRequest<IResult>
{
    public List<UpdateModelCommand> ModifiedModels { get; set; }
}

public class UpdateManyModelsCommandHandler : IRequestHandler<UpdateManyModelsCommand, IResult> {
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateManyModelsCommandHandler(
        IAuthService authService,
        IDBService dbService,
        ICrudService crudService,
        ILogger<Brand> logger
    )
    {
        _authService = authService;
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
        var oldModels = await _crudService.GetByIdsAsync<Model>(command.ModifiedModels.Select(p => p.Id).ToList());

        // get existedModelsTitles and modifiedModelsTitles and changedModelsTitles
        var existedModelsTitles = oldModels.Select(p => p.Title);
        var modifiedModelsTitles = command.ModifiedModels.Select(p => p.ModifiedEntity.Title);
        var changedModelsTitles = modifiedModelsTitles
            .Where(title => !existedModelsTitles.Contains(title))
            .ToList();
        // if any titles changed check if already existed in db and then reject all inputs
        if (changedModelsTitles.Count > 0)
        {
            var modelsWithSameTitle = await _dbService.GetListAsync<Model>(p => changedModelsTitles.Contains(p.Title));
            if (modelsWithSameTitle.Count > 0)
            {
                _errorMessage = $"Models List was rejected, Some Titles are already existed.";
                _logger.LogError(_errorMessage);
                Results.Conflict(_errorMessage);
            }
        }

        // get existedProductIds and modifiedProductIds and changedProductIds
        var existedProductIds = oldModels.Select(p => p.ProductId);
        var modifiedProductIds = command.ModifiedModels.Select(p => p.ModifiedEntity.ProductId);
        var changedProductIds = modifiedProductIds
            .Where(id => !existedProductIds.Contains(id))
            .ToList();
        // if any ProductId changed check if it is not existed in db and then reject all inputs
        if (changedProductIds.Count > 0)
        {
            var existedProducts = await _dbService.GetListAsync<Product>(p => changedProductIds.Contains(p.Id));
            if (existedProducts.Count != changedProductIds.Count)
            {
                _errorMessage = $"one or more ProductId are not existed.";
                _logger.LogError(_errorMessage);
                Results.NotFound(_errorMessage);
            }
        }

        // get existedBrandIds and modifiedBrandIds and changedBrandIds
        var existedBrandIds = oldModels.Select(p => p.BrandId);
        var modifiedBrandIds = command.ModifiedModels.Select(p => p.ModifiedEntity.BrandId);
        var changedBrandIds = modifiedBrandIds
            .Where(id => !existedBrandIds.Contains(id))
            .ToList();
        // if any BrandId changed check if it is not existed in db and then reject all inputs
        if (changedBrandIds.Count > 0)
        {
            var existedBrands = await _dbService.GetListAsync<Brand>(p => changedBrandIds.Contains(p.Id));
            if (existedBrands.Count != changedBrandIds.Count)
            {
                _errorMessage = $"one or more BrandId are not existed.";
                _logger.LogError(_errorMessage);
                Results.NotFound(_errorMessage);
            }
        }

        // do the normal update many items action
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        var updatedModels = await _crudService.UpdateManyAsync<Model, ModelDto, UpdateModelCommand, AddModelCommand>(command.ModifiedModels, oldModels, loggedUserEmail ?? "app_dev");
        return Results.Ok(updatedModels);
    }

}
