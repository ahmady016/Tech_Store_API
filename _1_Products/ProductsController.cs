using MediatR;
using Microsoft.AspNetCore.Mvc;
using Products.Commands;
using Products.Queries;

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
    public Task<IResult> List(string type, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListProductsQuery() { ListType = type, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Products/Search?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of ProductDto</returns>
    [HttpGet]
    public Task<IResult> Search(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchProductsQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Products/Find/[id]
    /// </summary>
    /// <returns>ProductDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> FindOne(Guid id)
    {
        return _mediator.Send(new FindProductQuery() { Id = id });
    }
    /// <summary>
    /// Products/Find/[ids]
    /// </summary>
    /// <returns>List of ProductDto</returns>
    [HttpGet("{ids}")]
    public Task<IResult> FindList(string ids)
    {
        return _mediator.Send(new FindProductsQuery() { Ids = ids });
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