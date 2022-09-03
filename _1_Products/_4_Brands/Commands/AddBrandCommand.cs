using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Brands.Commands;

public class AddBrandCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Title is Required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must between 5 and 100 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is Required")]
    [StringLength(400, MinimumLength = 10, ErrorMessage = "DescriptionAr must between 10 and 400 characters")]
    public string Description { get; set; }

    [DataType(DataType.Url)]
    [Required(ErrorMessage = "Url is Required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Url must between 10 and 500 characters")]
    public string LogoUrl { get; set; }
}

public class AddBrandCommandHandler : IRequestHandler<AddBrandCommand, IResult>
{
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public AddBrandCommandHandler(
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
        AddBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        // check if the title are existed in db then reject the command and return error
        var existedBrand = _dbService.GetOne<Brand>(p => p.Title == command.Title);
        if (existedBrand is not null)
        {
            _errorMessage = $"Brand Title already existed.";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // do the normal Add action
        var createdBrand = _crudService.Add<Brand, BrandDto, AddBrandCommand>(command);
        return await Task.FromResult(Results.Ok(createdBrand));
    }

}
