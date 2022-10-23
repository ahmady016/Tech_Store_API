using AutoMapper;
using MediatR;

using DB;
using Entities;
using Dtos;
using Auth;

namespace Models.Commands;

public class AddManyModelsCommand : IRequest<IResult>
{
    public List<AddModelCommand> NewModels { get; set; }
}

public class AddManyModelsCommandHandler : IRequestHandler<AddManyModelsCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public AddManyModelsCommandHandler(
        IAuthService authService,
        IDBService dbService,
        ICrudService crudService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Product> logger
    )
    {
        _authService = authService;
        _dbService = dbService;
        _crudService = crudService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        AddManyModelsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get all titles
        var titles = command.NewModels.Select(e => e.Title).ToList();
        // check if any title are existed in db then reject the command and return error
        var existedModels = await _dbService.GetListAsync<Model>(p => titles.Contains(p.Title));
        if (existedModels.Count > 0)
        {
            _errorMessage = $"Some of NewModels Titles already existed.";
            _logger.LogError(_errorMessage);
            Results.Conflict(_errorMessage);
        }

        // get all productIds
        var productIds = command.NewModels.Select(e => e.ProductId).ToList();
        // check if any ProductId is not existed in db then reject the command and return error
        var existedProducts = await _dbService.GetListAsync<Product>(p => productIds.Contains(p.Id));
        if (existedProducts.Count != command.NewModels.Count)
        {
            _errorMessage = $"one or more ProductId are not existed.";
            _logger.LogError(_errorMessage);
            Results.NotFound(_errorMessage);
        }

        // get all brandIds
        var brandIds = command.NewModels.Select(e => e.BrandId).ToList();
        // check if any BrandId is not existed in db then reject the command and return error
        var existedBrands = await _dbService.GetListAsync<Brand>(p => brandIds.Contains(p.Id));
        if (existedBrands.Count != command.NewModels.Count)
        {
            _errorMessage = $"one or more BrandId are not existed.";
            _logger.LogError(_errorMessage);
            Results.NotFound(_errorMessage);
        }

        // do the normal Add action
        var newModels = _mapper.Map<List<Model>>(command.NewModels);
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        _dbService.AddRange<Model>(newModels, loggedUserEmail ?? "app_dev");
        // create models stocks with default values
        _dbCommandService.AddRange<Stock>(newModels.Select(model => new Stock() { ModelId = model.Id }).ToList());
        // save models and stocks to db
        await _dbCommandService.SaveChangesAsync();

        // map to modelsDto and return it
        var modelsDto = _mapper.Map<List<ModelDto>>(newModels);
        return Results.Ok(modelsDto);
    }

}
