using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Sales.Queries;

public class FindSaleQuery : IdInput {}

public class FindSaleQueryHandler : IRequestHandler<FindSaleQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<Sale> _logger;
    private string _errorMessage;
    public FindSaleQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper,
        ILogger<Sale> logger
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        FindSaleQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedSale = await _dbQueryService.GetQuery<Sale>()
            .Include(e => e.Items)
            .Where(e => e.Id == request.Id)
            .FirstOrDefaultAsync();

        if(existedSale is null)
        {
            _errorMessage = $"Sale Record with Id: {request.Id} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        return Results.Ok(_mapper.Map<SaleDto>(existedSale));
    }

}
