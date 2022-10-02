using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Purchases.Commands;

public class CreatePurchaseItemInput
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }

    [Required(ErrorMessage = "Quantity is Required")]
    [MinLength(1)]
    public int Quantity { get; set; } = 0;

    [Required(ErrorMessage = "UnitPrice is Required")]
    [MinLength(1)]
    public double UnitPrice { get; set; } = 0.0;

    [MinLength(1)]
    public double TotalPrice { get; set; } = 0.0;
}
public class CreatePurchaseWithItemsCommand : IRequest<IResult>
{
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "PurchasedAt is Required")]
    public DateTime PurchasedAt { get; set; }

    [MinLength(1)]
    public double TotalPrice { get; set; }

    [Required(ErrorMessage = "EmployeeId is Required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "EmployeeId must between 10 and 450 characters")]
    public string EmployeeId { get; set; }

    [Required(ErrorMessage = "Items is Required")]
    public List<CreatePurchaseItemInput> Items { get; set; }
}

public class CreatePurchaseWithItemsCommandHandler : IRequestHandler<CreatePurchaseWithItemsCommand, IResult>
{
    private readonly IDBCommandService _dbCommandService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public CreatePurchaseWithItemsCommandHandler(
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
        CreatePurchaseWithItemsCommand command,
        CancellationToken cancellationToken
    )
    {
        // calc each item totalPrice if not provided
        foreach (var item in command.Items)
            if (item.TotalPrice == 0.0)
                item.TotalPrice = item.UnitPrice * item.Quantity;

        // calc purchase totalPrice if not provided
        if(command.TotalPrice == 0.0)
            command.TotalPrice = command.Items.Aggregate(0.0, (total, item) => total + item.TotalPrice);

        // create the new purchase and save it with its items to db
        var newPurchase = _mapper.Map<Purchase>(command);
        _dbCommandService.Add<Purchase>(newPurchase);
        await _dbCommandService.SaveChangesAsync();

        // update each model stock values and save it to db
        var modelIds = newPurchase.Items.Select(e => e.ModelId).ToList();
        var stocks = await _dbQueryService.GetListAsync<Stock>(e => modelIds.Contains(e.ModelId));
        foreach (var stock in stocks)
        {
            var purchaseItem = newPurchase.Items.FirstOrDefault(e => e.ModelId == stock.ModelId);
            if(purchaseItem is not null)
                stock.PurchaseUpdate(purchaseItem.Quantity, purchaseItem.TotalPrice);
        }
        _dbCommandService.UpdateRange<Stock>(stocks);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(_mapper.Map<PurchaseDto>(newPurchase));
    }

}
