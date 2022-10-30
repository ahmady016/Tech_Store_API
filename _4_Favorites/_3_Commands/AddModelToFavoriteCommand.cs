using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;
using Auth;

namespace Favorites.Commands;

public class AddModelToFavoriteCommand : CustomerFavoriteModelInput {}

public class AddModelToFavoriteCommandHandler : IRequestHandler<AddModelToFavoriteCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public AddModelToFavoriteCommandHandler(
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
        AddModelToFavoriteCommand command,
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

        var existedModel = await _dbQueryService.FindAsync<Model>(command.ModelId);
        if(existedModel is null)
        {
            _errorMessage = $"Model Record with Id: {command.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var newFavorite = _mapper.Map<CustomerFavoriteModel>(command);
        _dbCommandService.Add<CustomerFavoriteModel>(newFavorite);
        await _dbCommandService.SaveChangesAsync();

        newFavorite.Customer = existedCustomer;
        newFavorite.Model = existedModel;
        var favoriteDto = _mapper.Map<CustomerFavoriteModelDto>(newFavorite);
        return Results.Ok(favoriteDto);
    }
}
