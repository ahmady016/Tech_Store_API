using AutoMapper;
using MediatR;

using DB;
using Entities;
using Auth;

namespace Favorites.Commands;

public class UpdateCustomerFavoriteModelsCommand : IRequest<IResult>
{
    public List<Guid> ModelsToAdd { get; set; }
    public List<Guid> ModelsToRemove { get; set; }
}

public class UpdateCustomerFavoriteModelsCommandHandler : IRequestHandler<UpdateCustomerFavoriteModelsCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public UpdateCustomerFavoriteModelsCommandHandler(
        IAuthService authService,
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<CustomerFavoriteModel> logger
    )
    {
        _authService = authService;
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
        // get current logged user from auth
        var existedCustomer = await _authService.GetCurrentUser();
        if(existedCustomer is null)
        {
            _errorMessage = $"Signin required";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        if(command.ModelsToAdd is not null && command.ModelsToAdd.Count > 0)
        {
            var customerFavoriteModels = command.ModelsToAdd
                .Select(modelId => new CustomerFavoriteModel() { CustomerId = existedCustomer.Id, ModelId = modelId })
                .ToList();
            _dbCommandService.AddRange<CustomerFavoriteModel>(customerFavoriteModels);
            await _dbCommandService.SaveChangesAsync();
        }

        if(command.ModelsToRemove is not null && command.ModelsToRemove.Count > 0)
        {
            var customerFavoriteModels = command.ModelsToRemove
                .Select(modelId => new CustomerFavoriteModel() { CustomerId = existedCustomer.Id, ModelId = modelId })
                .ToList();
            _dbCommandService.RemoveRange<CustomerFavoriteModel>(customerFavoriteModels);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(new { Message = $"Customer [with Id: {existedCustomer.Id}] Favorite Models Updated successfully ..." });
    }
}
