using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;

namespace Sales.Commands;

public class RemoveSaleWithItemsCommand : IdInput {}

public class RemoveSaleWithItemsCommandHandler : IRequestHandler<RemoveSaleWithItemsCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public RemoveSaleWithItemsCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Brand> logger
    )
    {
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        RemoveSaleWithItemsCommand command,
        CancellationToken cancellationToken
    )
    {
        // find existedSale with its items
        var existedSale = await _dbQueryService.GetQuery<Sale>()
            .Include(e => e.Items)
            .Where(e => e.Id == command.Id)
            .FirstOrDefaultAsync();

        if(existedSale is null)
        {
            _errorMessage = $"Sale Record with Id: {command.Id} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        // remove existedSale with its items
        _dbCommandService.Remove<Sale>(existedSale);
        _dbCommandService.RemoveRange<SaleItem>(existedSale.Items.ToList());
        await _dbCommandService.SaveChangesAsync();

        // update each model stock values and save it to db
        var modelIds = existedSale.Items.Select(e => e.ModelId).ToList();
        var stocks = await _dbQueryService.GetListAsync<Stock>(e => modelIds.Contains(e.ModelId));
        foreach (var stock in stocks)
        {
            var saleItem = existedSale.Items.FirstOrDefault(e => e.ModelId == stock.ModelId);
            if(saleItem is not null)
                stock.SaleUpdate(-saleItem.Quantity, -saleItem.TotalPrice);
        }
        _dbCommandService.UpdateRange<Stock>(stocks);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(new { Message = $"Sale Record with Id: {command.Id} with it's Items was Removed Successfully" });
    }
}
