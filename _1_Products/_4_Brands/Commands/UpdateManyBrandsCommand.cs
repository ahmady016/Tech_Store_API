using System.Net;
using MediatR;

using DB;
using Entities;
using Dtos;

namespace Brands.Commands;

public class UpdateManyBrandsCommand : IRequest<IResult>
{
    public List<UpdateBrandCommand> ModifiedBrands { get; set; }
}

public class UpdateManyBrandsCommandHandler : IRequestHandler<UpdateManyBrandsCommand, IResult> {
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateManyBrandsCommandHandler(
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
        UpdateManyBrandsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get oldBrands from db
        var oldBrands = _crudService.GetByIds<Brand>(command.ModifiedBrands.Select(p => p.Id).ToList());

        // get existedBrandsTitles and modifiedBrandsTitles
        var existedBrandsTitles = oldBrands.Select(p => p.Title);
        var modifiedBrandsTitles = command.ModifiedBrands.Select(p => p.Title);
        // get changedBrandsTitles
        var changedBrandsTitles = modifiedBrandsTitles
            .Where(title => !existedBrandsTitles.Contains(title))
            .ToList();
        // if any titles changed check if already existed in db and then reject all inputs
        if (changedBrandsTitles.Count > 0)
        {
            var brandsWithSameTitle = _dbService.GetList<Brand>(p => changedBrandsTitles.Contains(p.Title));
            if (brandsWithSameTitle.Count > 0)
            {
                _errorMessage = $"Brands List was rejected, Some Titles are already existed.";
                _logger.LogError(_errorMessage);
                throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
            }
        }

        // do the normal update many items action
        var updatedBrands = _crudService.UpdateMany<Brand, BrandDto, UpdateBrandCommand>(command.ModifiedBrands, oldBrands);
        return await Task.FromResult(Results.Ok(updatedBrands));
    }

}