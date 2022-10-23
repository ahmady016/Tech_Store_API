using MediatR;

using DB;
using Entities;
using Dtos;
using Common;
using Auth;

namespace Brands.Commands;

public class UpdateBrandCommand : UpdateCommand<AddBrandCommand> {}

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, IResult> {
    private readonly IAuthService _authService;
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateBrandCommandHandler(
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
        UpdateBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existed db item
        var oldBrand = await _crudService.GetByIdAsync<Brand>(command.Id);
        // if title changed
        if(oldBrand.Title != command.ModifiedEntity.Title)
        {
            // check if the title are existed in db then reject the command and return error
            var brandWithSameTitle = await _dbService.GetOneAsync<Brand>(e => e.Title == command.ModifiedEntity.Title);
            if (brandWithSameTitle is not null)
            {
                _errorMessage = $"Brand Title is already existed.";
                _logger.LogError(_errorMessage);
                return Results.Conflict(_errorMessage);
            }
        }

        // do the normal update action
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        var updatedBrand = await _crudService.UpdateAsync<Brand, BrandDto, UpdateBrandCommand, AddBrandCommand>(command, oldBrand, loggedUserEmail ?? "app_dev");
        return Results.Ok(updatedBrand);
    }

}
