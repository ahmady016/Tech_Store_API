using MediatR;

using DB;
using Entities;
using Dtos;
using Common;

namespace Products.Commands;

public class UpdateProductCommand : UpdateInputBase
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, IResult> {
    private readonly ICrudService _crudService;
    public UpdateProductCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        UpdateProductCommand input,
        CancellationToken cancellationToken
    )
    {
        var updatedProduct = _crudService.Update<Product, ProductDto, UpdateProductCommand>(input);
        return await Task.FromResult(Results.Ok(updatedProduct));
    }

}
