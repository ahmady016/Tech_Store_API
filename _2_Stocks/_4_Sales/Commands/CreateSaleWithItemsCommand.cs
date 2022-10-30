using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;
using Auth;

namespace Sales.Commands;

public class CreateSaleWithItemsCommand : IRequest<IResult>
{
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "SoldAt is Required")]
    public DateTime SoldAt { get; set; }

    [MinLength(1)]
    public double TotalPrice { get; set; }

    [Required(ErrorMessage = "CustomerId is Required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "CustomerId must between 10 and 450 characters")]
    public string CustomerId { get; set; }

    [Required(ErrorMessage = "Items is Required")]
    public List<CreateSaleItemCommand> Items { get; set; }
}

public class CreateSaleWithItemsCommandHandler : IRequestHandler<CreateSaleWithItemsCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<Sale> _logger;
    private string _errorMessage;

    public CreateSaleWithItemsCommandHandler(
        IAuthService authService,
        IDBCommandService dbCommandService,
        IDBQueryService dbQueryService,
        IMapper mapper,
        ILogger<Sale> logger
    )
    {
        _authService = authService;
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        CreateSaleWithItemsCommand command,
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

        // calc Sale totalPrice if not provided
        if(command.TotalPrice == 0.0)
            command.TotalPrice = command.Items.Aggregate(0.0, (total, item) => total + item.TotalPrice);

        // create the new Sale and save it with its items to db
        var newSale = _mapper.Map<Sale>(command);
        newSale.EmployeeId = userId;
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
