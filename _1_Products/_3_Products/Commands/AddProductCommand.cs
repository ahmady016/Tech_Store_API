using System.Net;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Products.Commands;

public class AddProductCommand : IRequest<IResult>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
}

public class AddProductHandler : IRequestHandler<AddProductCommand, IResult>
{
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public AddProductHandler(
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
        AddProductCommand input,
        CancellationToken cancellationToken
    )
    {
        // check if the title are existed in db then reject the command and return error
        var existedProduct = _dbService.GetOne<Product>(p => p.Title == input.Title);
        if (existedProduct is not null)
        {
            _errorMessage = $"Product Title already existed.";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // do the normal Add action
        var createdProduct = _crudService.Add<Product, ProductDto, AddProductCommand>(input);
        return await Task.FromResult(Results.Ok(createdProduct));
    }

}
