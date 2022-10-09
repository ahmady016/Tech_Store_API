using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Favorites.Commands;

public class RemoveModelFromFavoriteCommand : CustomerFavoriteModelInput {}

public class RemoveModelFromFavoriteCommandHandler : IRequestHandler<RemoveModelFromFavoriteCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public RemoveModelFromFavoriteCommandHandler(
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
        RemoveModelFromFavoriteCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedFavorite = await _dbQueryService.GetOneAsync<CustomerFavoriteModel>(e => e.CustomerId == command.CustomerId && e.ModelId == command.ModelId);
        if(existedFavorite is null)
        {
            _errorMessage = $"CustomerFavoriteModel Record with CustomerId: {command.CustomerId} and ModelId: {command.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        _dbCommandService.Remove<CustomerFavoriteModel>(existedFavorite);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(new { Message = $"CustomerFavoriteModel Record with CustomerId: {command.CustomerId} and ModelId: {command.ModelId} was Removed Successfully ..." });
    }
}
