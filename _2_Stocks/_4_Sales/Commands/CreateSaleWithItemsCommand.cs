using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Sales.Commands;

public class CreateSaleWithItemsCommand : IRequest<IResult>
{
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "SoldAt is Required")]
    public DateTime SoldAt { get; set; }

    [MinLength(1)]
    public double TotalPrice { get; set; }

    [Required(ErrorMessage = "EmployeeId is Required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "EmployeeId must between 10 and 450 characters")]
    public string EmployeeId { get; set; }

    [Required(ErrorMessage = "CustomerId is Required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "CustomerId must between 10 and 450 characters")]
    public string CustomerId { get; set; }

    [Required(ErrorMessage = "Items is Required")]
    public List<CreateSaleItemCommand> Items { get; set; }
}

public class CreateSaleWithItemsCommandHandler : IRequestHandler<CreateSaleWithItemsCommand, IResult>
{
    private readonly IDBCommandService _dbCommandService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public CreateSaleWithItemsCommandHandler(
        IDBCommandService dbCommandService,
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        CreateSaleWithItemsCommand command,
        CancellationToken cancellationToken
    )
    {
        // calc each item totalPrice if not provided
        foreach (var item in command.Items)
            if (item.TotalPrice == 0.0)
                item.TotalPrice = item.UnitPrice * item.Quantity;

        // calc Sale totalPrice if not provided
        if(command.TotalPrice == 0.0)
            command.TotalPrice = command.Items.Aggregate(0.0, (total, item) => total + item.TotalPrice);

        // create the new Sale and save it with its items to db
        var newSale = _mapper.Map<Sale>(command);
        _dbCommandService.Add<Sale>(newSale);
        await _dbCommandService.SaveChangesAsync();

        // update each model stock values and save it to db
        var modelIds = newSale.Items.Select(e => e.ModelId).ToList();
        var stocks = await _dbQueryService.GetListAsync<Stock>(e => modelIds.Contains(e.ModelId));
        foreach (var stock in stocks)
        {
            var saleItem = newSale.Items.FirstOrDefault(e => e.ModelId == stock.ModelId);
            if(saleItem is not null)
                stock.SaleUpdate(saleItem.Quantity, saleItem.TotalPrice);
        }
        _dbCommandService.UpdateRange<Stock>(stocks);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(_mapper.Map<SaleDto>(newSale));
    }

}
