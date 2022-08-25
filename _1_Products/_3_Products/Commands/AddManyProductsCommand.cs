using DB;
using Dtos;
using Entities;
using MediatR;

namespace Products.Commands;
public class AddManyProductsCommand : IRequest<IResult>
{
  public List<AddProductCommand> NewProducts { get; set; }
}

public class AddManyProductsCommandHandler : IRequestHandler<AddManyProductsCommand, IResult>
{
  private readonly ICrudService _crudService;
  public AddManyProductsCommandHandler(ICrudService crudService)
  {
    _crudService = crudService;
  }

  public async Task<IResult> Handle(
    AddManyProductsCommand command,
    CancellationToken cancellationToken
  )
  {
    var createdProducts = _crudService.AddMany<Product, ProductDto, AddProductCommand>(command.NewProducts);
    return await Task.FromResult(Results.Ok(createdProducts));
  }

}
