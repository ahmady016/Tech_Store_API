using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;

using DB;
using Dtos;
using Entities;

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
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Model> _logger;
    private string _errorMessage;
    public AddModelCommandHandler(
        IDBService dbService,
        ICrudService crudService,
        ILogger<Model> logger
    )
    {
        _dbService = dbService;
        _crudService = crudService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        AddModelCommand input,
        CancellationToken cancellationToken
    )
    {
        // check if the title are existed in db then reject the command and return error
        var existedModel = _dbService.GetOne<Model>(p => p.Title == input.Title);
        if (existedModel is not null)
        {
            _errorMessage = $"Model Title already existed.";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // do the normal Add action
        var createdModel = _crudService.Add<Model, ModelDto, AddModelCommand>(input);
        return await Task.FromResult(Results.Ok(createdModel));
    }

}
