using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Entities;
using Dtos;
using Auth;

namespace Ratings.Commands;

public class UpdateRatingCommand : RatingInput {}

public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Rating> _logger;
    private string _errorMessage;
    public UpdateRatingCommandHandler(
        IAuthService authService,
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Rating> logger
    )
    {
        _authService = authService;
        _dbQueryService = dbQueryService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateRatingCommand command,
        CancellationToken cancellationToken
    )
    {
        // get current logged user from auth
        var customerId = _authService.GetCurrentUserId();
        if(customerId is null)
        {
            _errorMessage = $"Signin required";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        var existedRating = await _dbQueryService
            .GetQuery<Rating>(e => e.CustomerId == customerId && e.ModelId == command.ModelId)
            .Include("Customer")
            .FirstOrDefaultAsync();
        if(existedRating is null)
        {
            _errorMessage = $"Rating Record with CustomerId: {customerId} and ModelId: {command.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var existedModel = await _dbQueryService
            .GetQuery<Model>(e => e.Id == command.ModelId)
            .Include("Ratings")
            .FirstOrDefaultAsync();
        if(existedModel is null)
        {
            _errorMessage = $"Model Record with Id: {command.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        // calc the diff rating value and recalculate the ratingAverage
        var diffRatingValue = command.Value - existedRating.Value;
        var totalRatingValue = existedModel.Ratings.Aggregate(0, (total, rating) => total + rating.Value) + diffRatingValue;
        existedModel.RatingAverage = totalRatingValue / existedModel.RatingCount;

        // update the model rating values
        existedModel.Ratings = null;
        _dbCommandService.Update<Model>(existedModel);

        // update the existedRating Value
        existedRating.Value = command.Value;
        _dbCommandService.Update<Rating>(existedRating);

        // save both updated model values and updated rating value to db
        await _dbCommandService.SaveChangesAsync();

        // get and return ratingDto
        existedRating.Model = existedModel;
        var ratingDto = _mapper.Map<Rating>(existedRating);
        return Results.Ok(ratingDto);
    }
}
