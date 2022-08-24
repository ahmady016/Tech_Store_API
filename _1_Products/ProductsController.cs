using MediatR;
using Microsoft.AspNetCore.Mvc;

using Products.Queries;
using Products.Commands;

namespace Products;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductsController : ControllerBase
{
  private readonly IMediator _mediator;
  public ProductsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// listType values (all/deleted/existed)
  /// Products/List?type=existed
  /// </summary>
  /// <returns>List of ProductDto</returns>
  [HttpGet]
  public Task<IResult> List([AsParameters] ListProductQuery query)
  {
      return _mediator.Send(query);
  }
  /// <summary>
  /// Products/Find/[id]
  /// </summary>
  /// <returns>ProductDto</returns>
  [HttpGet("{id}")]
  public Task<IResult> Find([AsParameters] FindProductQuery query)
  {
      return _mediator.Send(query);
  }

  /// <summary>
  /// Products/Add
  /// </summary>
  /// <returns>ProductDto</returns>
  [HttpPost]
  public Task<IResult> Add(AddProductCommand input)
  {
      return _mediator.Send(input);
  }
  /// <summary>
  /// Products/Update
  /// </summary>
  /// <returns>ProductDto</returns>
  [HttpPost]
  public Task<IResult> Update(UpdateProductCommand input)
  {
      return _mediator.Send(input);
  }

}
