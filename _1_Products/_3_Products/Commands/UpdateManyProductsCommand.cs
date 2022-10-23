using MediatR;

using DB;
using Entities;
using Dtos;
using Auth;

namespace Products.Commands;

public class UpdateManyProductsCommand : IRequest<IResult>
{
    public List<UpdateProductCommand> ModifiedProducts { get; set; }
}

public class UpdateManyProductsCommandHandler : IRequestHandler<UpdateManyProductsCommand, IResult> {
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateManyProductsCommandHandler(
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
        UpdateManyProductsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get oldProducts from db
        var oldProducts = await _crudService.GetByIdsAsync<Product>(command.ModifiedProducts.Select(p => p.Id).ToList());

        // get existedProductsTitles and modifiedProductsTitles
        var existedProductsTitles = oldProducts.Select(p => p.Title);
        var modifiedProductsTitles = command.ModifiedProducts.Select(p => p.ModifiedEntity.Title);
        // get changedProductsTitles
        var changedProductsTitles = modifiedProductsTitles
            .Where(title => !existedProductsTitles.Contains(title))
            .ToList();
        // if any titles changed check if already existed in db and then reject all inputs
        if (changedProductsTitles.Count > 0)
        {
            var productsWithSameTitle = await _dbService.GetListAsync<Product>(p => changedProductsTitles.Contains(p.Title));
            if (productsWithSameTitle.Count > 0)
            {
                _errorMessage = $"Products List was rejected, Some Titles are already existed.";
                _logger.LogError(_errorMessage);
                Results.Conflict(_errorMessage);
            }
        }

        // do the normal update many items action
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        var updatedProducts = await _crudService.UpdateManyAsync<Product, ProductDto, UpdateProductCommand, AddProductCommand>(command.ModifiedProducts, oldProducts, loggedUserEmail ?? "app_dev");
        return Results.Ok(updatedProducts);
    }

}
