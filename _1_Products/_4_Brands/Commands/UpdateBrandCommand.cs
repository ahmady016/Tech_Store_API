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
        var oldBrand = _crudService.GetById<Brand>(command.Id);
        // if title changed
        if(oldBrand.Title != command.ModifiedEntity.Title)
        {
            // check if the title are existed in db then reject the command and return error
            var brandWithSameTitle = _dbService.GetOne<Brand>(e => e.Title == command.ModifiedEntity.Title);
            if (brandWithSameTitle is not null)
            {
                _errorMessage = $"Brand Title is already existed.";
                _logger.LogError(_errorMessage);
                throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
            }
        }

        // do the normal update action
        var updatedBrand = _crudService.Update<Brand, BrandDto, UpdateBrandCommand, AddBrandCommand>(command, oldBrand);
        return await Task.FromResult(Results.Ok(updatedBrand));
    }

}
