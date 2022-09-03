using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Products.Commands;

public class AddProductCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Title is Required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must between 5 and 100 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is Required")]
    [StringLength(400, MinimumLength = 10, ErrorMessage = "DescriptionAr must between 10 and 400 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Category Category { get; set; }
}

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, IResult>
{
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public AddProductCommandHandler(
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
        AddProductCommand command,
        CancellationToken cancellationToken
    )
    {
        // check if the title are existed in db then reject the command and return error
        var existedProduct = _dbService.GetOne<Product>(p => p.Title == command.Title);
        if (existedProduct is not null)
        {
            _errorMessage = $"Product Title already existed.";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // do the normal Add action
        var createdProduct = _crudService.Add<Product, ProductDto, AddProductCommand>(command);
        return await Task.FromResult(Results.Ok(createdProduct));
    }

}
