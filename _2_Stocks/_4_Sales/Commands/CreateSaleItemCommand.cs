using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Sales.Commands;

public class CreateSaleItemCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }

    [Required(ErrorMessage = "Quantity is Required")]
    public int Quantity { get; set; } = 0;

    [Required(ErrorMessage = "UnitPrice is Required")]
    public double UnitPrice { get; set; } = 0.0;

    [MinLength(1)]
    public double TotalPrice { get; set; } = 0.0;
}

public class CreateSaleItemCommandHandler : IRequestHandler<CreateSaleItemCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    public CreateSaleItemCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        CreateSaleItemCommand command,
        CancellationToken cancellationToken
    )
    {
        // calc item totalPrice if not provided
        if (command.TotalPrice == 0.0)
            command.TotalPrice = command.UnitPrice * command.Quantity;

        // create the new SaleItem and save it to db
        var newSaleItem = _mapper.Map<SaleItem>(command);
        _dbCommandService.Add<SaleItem>(newSaleItem);
        await _dbCommandService.SaveChangesAsync();

        // update model stock values and save it to db
        var stock = await _dbQueryService.GetOneAsync<Stock>(e => e.ModelId == newSaleItem.ModelId);
        if(stock is not null)
        {
            stock.SaleUpdate(newSaleItem.Quantity, newSaleItem.TotalPrice);
            _dbCommandService.Update<Stock>(stock);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(_mapper.Map<SaleItemDto>(newSaleItem));
    }

}
