using System.Net;
using MediatR;

using DB;
using Entities;
using Dtos;
using Common;

namespace Brands.Commands;

public class UpdateBrandCommand : UpdateCommand<AddBrandCommand> {}

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, IResult> {
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public UpdateBrandCommandHandler(
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
        var updatedBrand = await _crudService.UpdateAsync<Brand, BrandDto, UpdateBrandCommand, AddBrandCommand>(command, oldBrand);
        return Results.Ok(updatedBrand);
    }

}
