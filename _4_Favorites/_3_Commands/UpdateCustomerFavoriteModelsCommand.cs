using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Entities;

namespace Favorites.Commands;

public class UpdateCustomerFavoriteModelsCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "CustomerId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "CustomerId Must between 10 and 450 characters")]
    public string CustomerId { get; set; }
    public List<Guid> ModelsToAdd { get; set; }
    public List<Guid> ModelsToRemove { get; set; }
}

public class UpdateCustomerFavoriteModelsCommandHandler : IRequestHandler<UpdateCustomerFavoriteModelsCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public UpdateCustomerFavoriteModelsCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<CustomerFavoriteModel> logger
    )
    {
        _dbQueryService = dbQueryService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<IResult> Handle(
        UpdateCustomerFavoriteModelsCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedCustomer = await _dbQueryService.FindAsync<User>(command.CustomerId);
        if(existedCustomer is null)
        {
            _errorMessage = $"Customer Record with Id: {command.CustomerId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        if(command.ModelsToAdd is not null && command.ModelsToAdd.Count > 0)
        {
            var customerFavoriteModels = command.ModelsToAdd
                .Select(modelId => new CustomerFavoriteModel() { CustomerId = command.CustomerId, ModelId = modelId })
                .ToList();
            _dbCommandService.AddRange<CustomerFavoriteModel>(customerFavoriteModels);
            await _dbCommandService.SaveChangesAsync();
        }

        if(command.ModelsToRemove is not null && command.ModelsToRemove.Count > 0)
        {
            var customerFavoriteModels = command.ModelsToRemove
                .Select(modelId => new CustomerFavoriteModel() { CustomerId = command.CustomerId, ModelId = modelId })
                .ToList();
            _dbCommandService.RemoveRange<CustomerFavoriteModel>(customerFavoriteModels);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(new { Message = $"Customer [with Id: {command.CustomerId}] Favorite Models Updated successfully ..." });
    }
}
