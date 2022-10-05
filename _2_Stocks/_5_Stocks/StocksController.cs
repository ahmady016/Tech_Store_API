using MediatR;
using Microsoft.AspNetCore.Mvc;

using Stocks.Queries;

namespace Stocks;

[ApiController]
[Route("api/[controller]/[action]")]
public class StocksController : ControllerBase
{
    private readonly IMediator _mediator;
    public StocksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Stocks/List
    /// </summary>
    /// <returns>List of DetailedStockDto</returns>
    [HttpGet]
    public Task<IResult> List(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListStocksQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Stocks/Search?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of DetailedStockDto</returns>
    [HttpGet]
    public Task<IResult> Search(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchStocksQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Stocks/Find/[modelId]
    /// </summary>
    /// <returns>DetailedStockDto</returns>
    [HttpGet("{modelId}")]
    public Task<IResult> Find(Guid modelId)
    {
        return _mediator.Send(new FindStockQuery() { ModelId = modelId });
    }

}
