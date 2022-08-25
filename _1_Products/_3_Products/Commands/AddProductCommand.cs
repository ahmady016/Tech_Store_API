using DB;
using Dtos;
using Entities;
using MediatR;

namespace Products.Commands;

public class AddProductCommand : IRequest<IResult>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
}

public class AddProductHandler : IRequestHandler<AddProductCommand, IResult>
{
    private readonly ICrudService _crudService;
    public AddProductHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        AddProductCommand input,
        CancellationToken cancellationToken
    )
    {
        var createdProduct = _crudService.Add<Product, ProductDto, AddProductCommand>(input);
        return await Task.FromResult(Results.Ok(createdProduct));
    }

}
