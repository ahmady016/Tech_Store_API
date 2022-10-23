using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Entities;
using Dtos;
using Auth;

namespace Models.Commands;

public class AddModelCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Title is Required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must between 5 and 100 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is Required")]
    [StringLength(400, MinimumLength = 10, ErrorMessage = "DescriptionAr must between 10 and 400 characters")]
    public string Description { get; set; }

    [DataType(DataType.Url)]
    [Required(ErrorMessage = "ThumbUrl is Required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "ThumbUrl must between 10 and 500 characters")]
    public string ThumbUrl { get; set; }

    [DataType(DataType.Url)]
    [Required(ErrorMessage = "PhotoUrl is Required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "PhotoUrl must between 10 and 500 characters")]
    public string PhotoUrl { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Category Category { get; set; }

    [Required(ErrorMessage = "ProductId is required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ProductId value"
    )]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "BrandId is required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid BrandId value"
    )]
    public Guid BrandId { get; set; }
}

public class AddModelCommandHandler : IRequestHandler<AddModelCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Model> _logger;
    private string _errorMessage;
    public AddModelCommandHandler(
        IAuthService authService,
        IDBService dbService,
        ICrudService crudService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Model> logger
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
        AddModelCommand command,
        CancellationToken cancellationToken
    )
    {
        // check if the title are existed in db then reject the command and return error
        var existedModel = await _dbService.GetOneAsync<Model>(p => p.Title == command.Title);
        if (existedModel is not null)
        {
            _errorMessage = $"Model Title already existed.";
            _logger.LogError(_errorMessage);
            return Results.Conflict(_errorMessage);
        }

        // check if the product is not existed in db then reject the command and return error
        var existedProduct = await _dbService.GetOneAsync<Product>(p => p.Id == command.ProductId);
        if (existedProduct is null)
        {
            _errorMessage = $"Product with Id: {command.ProductId} not existed.";
            _logger.LogError(_errorMessage);
            return Results.NotFound(_errorMessage);
        }

        // check if the brand is not existed in db then reject the command and return error
        var existedBrand = await _dbService.GetOneAsync<Brand>(p => p.Id == command.BrandId);
        if (existedBrand is null)
        {
            _errorMessage = $"Brand with Id: {command.BrandId} not existed.";
            _logger.LogError(_errorMessage);
            return Results.NotFound(_errorMessage);
        }

        // do the normal Add action
        var newModel = _mapper.Map<Model>(command);
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        _dbService.Add<Model>(newModel, loggedUserEmail ?? "app_dev");
        // create model stock with default values
        _dbCommandService.Add<Stock>(new Stock() { ModelId = newModel.Id });
        // save model and stock to db
        await _dbCommandService.SaveChangesAsync();

        // map to modelDto and return it
        var modelDto = _mapper.Map<ModelDto>(newModel);
        return Results.Ok(modelDto);
    }

}
