using MediatR;

using DB;
using Entities;
using Dtos;
using Auth;

namespace Brands.Commands;

public class UpdateManyBrandsCommand : IRequest<IResult>
{
    public List<UpdateBrandCommand> ModifiedBrands { get; set; }
}

public class UpdateManyBrandsCommandHandler : IRequestHandler<UpdateManyBrandsCommand, IResult> {
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateManyBrandsCommandHandler(
        IAuthService authService,
        IDBService dbService,
        ICrudService crudService,
        ILogger<Brand> logger
    )
    {
        _authService = authService;
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
        var oldBrands = await _crudService.GetByIdsAsync<Brand>(command.ModifiedBrands.Select(p => p.Id).ToList());

        // get existedBrandsTitles and modifiedBrandsTitles
        var existedBrandsTitles = oldBrands.Select(p => p.Title);
        var modifiedBrandsTitles = command.ModifiedBrands.Select(p => p.ModifiedEntity.Title);
        // get changedBrandsTitles
        var changedBrandsTitles = modifiedBrandsTitles
            .Where(title => !existedBrandsTitles.Contains(title))
            .ToList();
        // if any titles changed check if already existed in db and then reject all inputs
        if (changedBrandsTitles.Count > 0)
        {
            var brandsWithSameTitle = await _dbService.GetListAsync<Brand>(p => changedBrandsTitles.Contains(p.Title));
            if (brandsWithSameTitle.Count > 0)
            {
                _errorMessage = $"Brands List was rejected, Some Titles are already existed.";
                _logger.LogError(_errorMessage);
                Results.Conflict(_errorMessage);
            }
        }

        // do the normal update many items action
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        var updatedBrands = await _crudService.UpdateManyAsync<Brand, BrandDto, UpdateBrandCommand, AddBrandCommand>(command.ModifiedBrands, oldBrands, loggedUserEmail ?? "app_dev");
        return Results.Ok(updatedBrands);
    }

}
