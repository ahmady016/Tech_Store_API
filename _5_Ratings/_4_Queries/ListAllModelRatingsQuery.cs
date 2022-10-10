using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using DB.Common;
using Entities;
using Dtos;

namespace Ratings.Queries;

public class ListAllModelRatingsQuery : IRequest<IResult>
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}

public class ListAllModelRatingsQueryHandler : IRequestHandler<ListAllModelRatingsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListAllModelRatingsQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListAllModelRatingsQuery query,
        CancellationToken cancellationToken
    )
    {
        var dbQuery = _dbQueryService
            .GetQuery<Rating>(e => e.ModelId == query.ModelId)
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
