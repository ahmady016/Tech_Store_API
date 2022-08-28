using System.Net;
using MediatR;

using DB;
using Entities;
using Dtos;

namespace Products.Commands;

public class UpdateManyProductsCommand : IRequest<IResult>
{
    public List<UpdateProductCommand> ModifiedProducts { get; set; }
}

public class UpdateManyProductsCommandHandler : IRequestHandler<UpdateManyProductsCommand, IResult> {
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateManyProductsCommandHandler(
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
        UpdateManyProductsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get oldProducts from db
        var oldProducts = _crudService.GetByIds<Product>(command.ModifiedProducts.Select(p => p.Id).ToList());

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
            var productsWithSameTitle = _dbService.GetList<Product>(p => changedProductsTitles.Contains(p.Title));
            if (productsWithSameTitle.Count > 0)
            {
                _errorMessage = $"Products List was rejected, Some Titles are already existed.";
                _logger.LogError(_errorMessage);
                throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
            }
        }

        // do the normal update many items action
        var updatedProducts = _crudService.UpdateMany<Product, ProductDto, UpdateProductCommand, AddProductCommand>(command.ModifiedProducts, oldProducts);
        return await Task.FromResult(Results.Ok(updatedProducts));
    }

}
