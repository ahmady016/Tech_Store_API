using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;
using Auth;

namespace Purchases.Commands;

public class CreatePurchaseWithItemsCommand : IRequest<IResult>
{
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "PurchasedAt is Required")]
    public DateTime PurchasedAt { get; set; }

    [MinLength(1)]
    public double TotalPrice { get; set; }

    [Required(ErrorMessage = "Items is Required")]
    public List<CreatePurchaseItemCommand> Items { get; set; }
}

public class CreatePurchaseWithItemsCommandHandler : IRequestHandler<CreatePurchaseWithItemsCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<Purchase> _logger;
    private string _errorMessage;
    public CreatePurchaseWithItemsCommandHandler(
        IAuthService authService,
        IDBCommandService dbCommandService,
        IDBQueryService dbQueryService,
        IMapper mapper,
        ILogger<Purchase> logger
    )
    {
        _authService = authService;
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        CreatePurchaseWithItemsCommand command,
        CancellationToken cancellationToken
    )
    {
        // get current logged userId from auth
        var userId = _authService.GetCurrentUserId();
        if(userId is null)
        {
            _errorMessage = $"Signin required";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        // calc each item totalPrice if not provided
        foreach (var item in command.Items)
            if (item.TotalPrice == 0.0)
                item.TotalPrice = item.UnitPrice * item.Quantity;

        // calc purchase totalPrice if not provided
        if(command.TotalPrice == 0.0)
            command.TotalPrice = command.Items.Aggregate(0.0, (total, item) => total + item.TotalPrice);

        // create the new purchase and save it with its items to db
        var newPurchase = _mapper.Map<Purchase>(command);
        newPurchase.EmployeeId = userId;
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
