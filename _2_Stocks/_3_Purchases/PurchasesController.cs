using MediatR;
using Microsoft.AspNetCore.Mvc;

using Purchases.Queries;
using Purchases.Commands;

namespace Purchases;

[ApiController]
[Route("api/[controller]/[action]")]
public class PurchasesController : ControllerBase
{
    private readonly IMediator _mediator;
    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Purchases/List
    /// </summary>
    /// <returns>List of PurchaseDto</returns>
    [HttpGet]
    public Task<IResult> List(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListPurchasesQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Purchases/Search?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of PurchaseDto</returns>
    [HttpGet]
    public Task<IResult> Search(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchPurchasesQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Purchases/Find/[id]
    /// </summary>
    /// <returns>PurchaseDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> Find(Guid id)
    {
        return _mediator.Send(new FindPurchaseQuery() { Id = id });
    }
    /// <summary>
    /// Purchases/ListItems
    /// </summary>
    /// <returns>List of DetailedPurchaseItemDto</returns>
    [HttpGet]
    public Task<IResult> ListItems(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListPurchasesItemsQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Purchases/SearchItems?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of DetailedPurchaseItemDto</returns>
    [HttpGet]
    public Task<IResult> SearchItems(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchPurchasesItemsQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }

    /// <summary>
    /// Purchases/Create
    /// </summary>
    /// <returns>PurchaseDto</returns>
    [HttpPost]
    public Task<IResult> Create(CreatePurchaseWithItemsCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Purchases/Remove
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> Remove(RemovePurchaseWithItemsCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Purchases/CreateItem
    /// </summary>
    /// <returns>PurchaseItemDto</returns>
    [HttpPost]
    public Task<IResult> CreateItem(CreatePurchaseItemCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Purchases/UpdateItem
    /// </summary>
    /// <returns>PurchaseItemDto</returns>
    [HttpPut]
    public Task<IResult> UpdateItem(UpdatePurchaseItemCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Purchases/RemoveItem
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> RemoveItem(RemovePurchaseItemCommand command)
    {
        return _mediator.Send(command);
    }

}
