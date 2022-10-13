using System.Net;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Brands.Commands;
public class AddManyBrandsCommand : IRequest<IResult>
{
    public List<AddBrandCommand> NewBrands { get; set; }
}

public class AddManyBrandsCommandHandler : IRequestHandler<AddManyBrandsCommand, IResult>
{
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public AddManyBrandsCommandHandler(
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
        AddManyBrandsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get all inputs titles
        var titles = command.NewBrands.Select(e => e.Title).ToList();
        // check if any title are existed in db then reject the command and return error
        var existedBrands = await _dbService.GetListAsync<Brand>(p => titles.Contains(p.Title));
        if (existedBrands.Count > 0)
        {
            _errorMessage = $"Some of NewBrands Titles already existed.";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // do the normal Add action
        var createdBrands = await _crudService.AddManyAsync<Brand, BrandDto, AddBrandCommand>(command.NewBrands);
        return Results.Ok(createdBrands);
    }

}
