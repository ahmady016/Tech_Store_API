using System.Net;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Brands.Commands;

public class AddBrandCommand : IRequest<IResult>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
}

public class AddBrandHandler : IRequestHandler<AddBrandCommand, IResult>
{
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public AddBrandHandler(
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
        AddBrandCommand input,
        CancellationToken cancellationToken
    )
    {
        // check if the title are existed in db then reject the command and return error
        var existedBrand = _dbService.GetOne<Brand>(p => p.Title == input.Title);
        if (existedBrand is not null)
        {
            _errorMessage = $"Brand Title already existed.";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // do the normal Add action
        var createdBrand = _crudService.Add<Brand, BrandDto, AddBrandCommand>(input);
        return await Task.FromResult(Results.Ok(createdBrand));
    }

}
