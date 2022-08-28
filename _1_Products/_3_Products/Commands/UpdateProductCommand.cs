using System.Net;
using MediatR;

using DB;
using Entities;
using Dtos;
using Common;

namespace Products.Commands;

public class UpdateProductCommand : IdInput
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, IResult> {
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateProductCommandHandler(
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
        UpdateProductCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existed db item
        var oldProduct = _crudService.GetById<Product>(command.Id);
        // if title changed
        if(oldProduct.Title != command.Title)
        {
            // check if the title are existed in db then reject the command and return error
            var productWithSameTitle = _dbService.GetOne<Product>(e => e.Title == command.Title);
            if (productWithSameTitle is not null)
            {
                _errorMessage = $"Product Title is already existed.";
                _logger.LogError(_errorMessage);
                throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
            }
        }
        // do the normal update action
        var updatedProduct = _crudService.Update<Product, ProductDto, UpdateProductCommand>(command, oldProduct);
        return await Task.FromResult(Results.Ok(updatedProduct));
    }

}
