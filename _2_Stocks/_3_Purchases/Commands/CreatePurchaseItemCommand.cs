using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Purchases.Commands;

public class CreatePurchaseItemCommand : IRequest<IResult>
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

public class CreatePurchaseItemCommandHandler : IRequestHandler<CreatePurchaseItemCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    public CreatePurchaseItemCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper
    )
    {
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        CreatePurchaseItemCommand command,
        CancellationToken cancellationToken
    )
    {
        // calc item totalPrice if not provided
        if (command.TotalPrice == 0.0)
            command.TotalPrice = command.UnitPrice * command.Quantity;

        // create the new purchaseItem and save it to db
        var newPurchaseItem = _mapper.Map<PurchaseItem>(command);
        _dbCommandService.Add<PurchaseItem>(newPurchaseItem);
        await _dbCommandService.SaveChangesAsync();

        // update model stock values and save it to db
        var stock = await _dbQueryService.GetOneAsync<Stock>(e => e.ModelId == newPurchaseItem.ModelId);
        if(stock is not null)
        {
            stock.PurchaseUpdate(newPurchaseItem.Quantity, newPurchaseItem.TotalPrice);
            _dbCommandService.Update<Stock>(stock);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(_mapper.Map<PurchaseItemDto>(newPurchaseItem));
    }

}
