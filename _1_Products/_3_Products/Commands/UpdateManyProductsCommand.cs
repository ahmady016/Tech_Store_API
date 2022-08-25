using MediatR;

using DB;
using Entities;
using Dtos;

namespace Products.Commands;

public class UpdateManyProductsCommand : IRequest<IResult>
{
    public List<UpdateProductCommand> ModifiedProducts { get; set; }
}

public class UpdateManyProductsCommandHandler : IRequestHandler<UpdateManyProductsCommand, IResult> {
    private readonly ICrudService _crudService;
    public UpdateManyProductsCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        UpdateManyProductsCommand command,
        CancellationToken cancellationToken
    )
    {
        var updatedProduct = _crudService.UpdateMany<Product, ProductDto, UpdateProductCommand>(command.ModifiedProducts);
        return await Task.FromResult(Results.Ok(updatedProduct));
    }

}
