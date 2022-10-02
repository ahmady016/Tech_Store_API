using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Purchases.Queries;

public class FindPurchaseQuery : IdInput {}

public class FindPurchaseQueryHandler : IRequestHandler<FindPurchaseQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<Purchase> _logger;
    private string _errorMessage;
    public FindPurchaseQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper,
        ILogger<Purchase> logger
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        FindPurchaseQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedPurchase = await _dbQueryService.GetQuery<Purchase>()
            .Include(e => e.Items)
            .Where(e => e.Id == request.Id)
            .FirstOrDefaultAsync();

        if(existedPurchase is null)
        {
            _errorMessage = $"Purchase Record with Id: {request.Id} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        return Results.Ok(_mapper.Map<PurchaseDto>(existedPurchase));
    }

}
