using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Sales.Queries;

public class ListSalesItemsQuery : ListQuery {}

public class ListSalesItemsQueryHandler : IRequestHandler<ListSalesItemsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListSalesItemsQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListSalesItemsQuery query,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        var dbQuery = _dbQueryService.GetQuery<SaleItem>()
                .Include(e => e.Sale)
                .Include(e => e.Model)
                .Select(item => new DetailedSaleItemDto()
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    ModelId = item.ModelId,
                    ModelTitle = item.Model.Title,
                    SaleId = item.SaleId,
                    SoldAt = item.Sale.SoldAt,
                    EmployeeId = item.Sale.EmployeeId
                });

        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService
                .GetPageAsync<DetailedSaleItemDto>(dbQuery, (int)query.PageSize, (int)query.PageNumber);
            result = Results.Ok(page);
        }
        else
        {
            var list = await dbQuery.ToListAsync();
            result = Results.Ok(list);
        }
        return result;
    }
}
