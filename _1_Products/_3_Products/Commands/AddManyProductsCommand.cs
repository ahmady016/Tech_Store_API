using MediatR;

using DB;
using Dtos;
using Entities;
using Auth;

namespace Products.Commands;
public class AddManyProductsCommand : IRequest<IResult>
{
    public List<AddProductCommand> NewProducts { get; set; }
}

public class AddManyProductsCommandHandler : IRequestHandler<AddManyProductsCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public AddManyProductsCommandHandler(
        IAuthService authService,
        IDBService dbService,
        ICrudService crudService,
        ILogger<Product> logger
    )
    {
        _authService = authService;
        _dbService = dbService;
        _crudService = crudService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        AddManyProductsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get all inputs titles
        var titles = command.NewProducts.Select(e => e.Title).ToList();
        // check if any title are existed in db then reject the command and return error
        var existedProducts = await _dbService.GetListAsync<Product>(p => titles.Contains(p.Title));
        if (existedProducts.Count > 0)
        {
            _errorMessage = $"Some of NewProducts Titles already existed.";
            _logger.LogError(_errorMessage);
            Results.Conflict(_errorMessage);
        }

        // do the normal Add action
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        var createdProducts = await _crudService.AddManyAsync<Product, ProductDto, AddProductCommand>(command.NewProducts, loggedUserEmail ?? "app_dev");
        return Results.Ok(createdProducts);
    }

}
