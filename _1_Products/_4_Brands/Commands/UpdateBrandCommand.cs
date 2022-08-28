using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;

using DB;
using Entities;
using Dtos;
using Common;

namespace Brands.Commands;

public class UpdateBrandCommand : IdInput
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
        if(oldBrand.Title != command.Title)
        {
            // check if the title are existed in db then reject the command and return error
            var brandWithSameTitle = _dbService.GetOne<Brand>(e => e.Title == command.Title);
            if (brandWithSameTitle is not null)
            {
                _errorMessage = $"Brand Title is already existed.";
                _logger.LogError(_errorMessage);
                throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
            }
        }

        // do the normal update action
        var updatedBrand = _crudService.Update<Brand, BrandDto, UpdateBrandCommand>(command, oldBrand);
        return await Task.FromResult(Results.Ok(updatedBrand));
    }

}
