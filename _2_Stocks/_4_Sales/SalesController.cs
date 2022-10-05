using MediatR;
using Microsoft.AspNetCore.Mvc;

using Sales.Queries;
using Sales.Commands;

namespace Sales;

[ApiController]
[Route("api/[controller]/[action]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sales/List
    /// </summary>
    /// <returns>List of SaleDto</returns>
    [HttpGet]
    public Task<IResult> List(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListSalesQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Sales/Search?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of SaleDto</returns>
    [HttpGet]
    public Task<IResult> Search(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchSalesQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Sales/Find/[id]
    /// </summary>
    /// <returns>SaleDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> Find(Guid id)
    {
        return _mediator.Send(new FindSaleQuery() { Id = id });
    }
    /// <summary>
    /// Sales/ListItems
    /// </summary>
    /// <returns>List of DetailedSaleItemDto</returns>
    [HttpGet]
    public Task<IResult> ListItems(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListSalesItemsQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Sales/SearchItems?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of DetailedSaleItemDto</returns>
    [HttpGet]
    public Task<IResult> SearchItems(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchSalesItemsQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }

    /// <summary>
    /// Sales/Create
    /// </summary>
    /// <returns>SaleDto</returns>
    [HttpPost]
    public Task<IResult> Create(CreateSaleWithItemsCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Sales/Remove
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> Remove(RemoveSaleWithItemsCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Sales/CreateItem
    /// </summary>
    /// <returns>SaleItemDto</returns>
    [HttpPost]
    public Task<IResult> CreateItem(CreateSaleItemCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Sales/UpdateItem
    /// </summary>
    /// <returns>SaleItemDto</returns>
    [HttpPut]
    public Task<IResult> UpdateItem(UpdateSaleItemCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Sales/RemoveItem
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> RemoveItem(RemoveSaleItemCommand command)
    {
        return _mediator.Send(command);
    }

}
