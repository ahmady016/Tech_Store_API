using MediatR;

using DB;
using Entities;
using Dtos;
using Common;
using Auth;

namespace Products.Commands;

public class UpdateProductCommand : UpdateCommand<AddProductCommand> {}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, IResult> {
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateProductCommandHandler(
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
        UpdateProductCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existed db item
        var oldProduct = await _crudService.GetByIdAsync<Product>(command.Id);
        // if title changed
        if(oldProduct.Title != command.ModifiedEntity.Title)
        {
            // check if the title are existed in db then reject the command and return error
            var productWithSameTitle = await _dbService.GetOneAsync<Product>(e => e.Title == command.ModifiedEntity.Title);
            if (productWithSameTitle is not null)
            {
                _errorMessage = $"Product Title is already existed.";
                _logger.LogError(_errorMessage);
                return Results.Conflict(_errorMessage);
            }
        }

        // do the normal update action
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        var updatedProduct = await _crudService.UpdateAsync<Product, ProductDto, UpdateProductCommand, AddProductCommand>(command, oldProduct, loggedUserEmail ?? "app_dev");
        return Results.Ok(updatedProduct);
    }

}
