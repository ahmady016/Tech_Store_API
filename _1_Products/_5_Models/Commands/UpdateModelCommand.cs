using MediatR;

using DB;
using Entities;
using Dtos;
using Common;
using Auth;

namespace Models.Commands;

public class UpdateModelCommand : UpdateCommand<AddModelCommand> {}

public class UpdateModelCommandHandler : IRequestHandler<UpdateModelCommand, IResult> {
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Model> _logger;
    private string _errorMessage;
    public UpdateModelCommandHandler(
        IAuthService authService,
        IDBService dbService,
        ICrudService crudService,
        ILogger<Model> logger
    )
    {
        _authService = authService;
        _dbService = dbService;
        _crudService = crudService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateModelCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existed db item
        var oldModel = await _crudService.GetByIdAsync<Model>(command.Id);

        // if title changed
        if(oldModel.Title != command.ModifiedEntity.Title)
        {
            // check if the title are existed in db then reject the command and return error
            var modelWithSameTitle = await _dbService.GetOneAsync<Model>(e => e.Title == command.ModifiedEntity.Title);
            if (modelWithSameTitle is not null)
            {
                _errorMessage = $"Model Title is already existed.";
                _logger.LogError(_errorMessage);
                return Results.Conflict(_errorMessage);
            }
        }

        // if productId changed
        if(oldModel.ProductId != command.ModifiedEntity.ProductId)
        {
            // check if the product is not existed in db then reject the command and return error
            var existedProduct = await _dbService.GetOneAsync<Product>(p => p.Id == command.ModifiedEntity.ProductId);
            if (existedProduct is null)
            {
                _errorMessage = $"Product with Id: {command.ModifiedEntity.ProductId} not existed.";
                _logger.LogError(_errorMessage);
                return Results.NotFound(_errorMessage);
            }
        }

        // if brandId changed
        if(oldModel.BrandId != command.ModifiedEntity.BrandId)
        {
            // check if the brand is not existed in db then reject the command and return error
            var existedBrand = await _dbService.GetOneAsync<Brand>(p => p.Id == command.ModifiedEntity.BrandId);
            if (existedBrand is null)
            {
                _errorMessage = $"Brand with Id: {command.ModifiedEntity.BrandId} not existed.";
                _logger.LogError(_errorMessage);
                return Results.NotFound(_errorMessage);
            }
        }

        // do the normal update action
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        var updatedModel = await _crudService.UpdateAsync<Model, ModelDto, UpdateModelCommand, AddModelCommand>(command, oldModel, loggedUserEmail ?? "app_dev");
        return Results.Ok(updatedModel);
    }

}
