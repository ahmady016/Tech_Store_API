using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using DB.Common;
using Entities;
using Dtos;

namespace Ratings.Queries;

public class ListAllRatingsQuery : IRequest<IResult>
{
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}

public class ListAllRatingsQueryHandler : IRequestHandler<ListAllRatingsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListAllRatingsQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListAllRatingsQuery query,
        CancellationToken cancellationToken
    )
    {
        var dbQuery = _dbQueryService
            .GetQuery<Rating>()
            .Include("Customer")
            .Include("Model")
            .Include("Model.Product")
            .Include("Model.Brand");

        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<Rating>(dbQuery, (int)query.PageSize, (int)query.PageNumber);
            return Results.Ok(new PageResult<RatingDto>()
            {
                PageItems = _mapper.Map<List<RatingDto>>(page.PageItems),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = await dbQuery.ToListAsync();
            return Results.Ok(_mapper.Map<List<RatingDto>>(list));
        }
    }
}
