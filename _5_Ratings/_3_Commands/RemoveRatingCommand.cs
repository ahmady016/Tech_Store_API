using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Entities;
using Auth;

namespace Ratings.Commands;

public class RemoveRatingCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
}

public class RemoveRatingCommandHandler : IRequestHandler<RemoveRatingCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Rating> _logger;
    private string _errorMessage;
    public RemoveRatingCommandHandler(
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
        RemoveRatingCommand command,
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
            .Include("Model")
            .Include("Model.Ratings")
            .FirstOrDefaultAsync();
        if(existedRating is null)
        {
            _errorMessage = $"Rating Record with CustomerId: {customerId} and ModelId: {command.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        // calc the ratingsCount and ratingAverage
        existedRating.Model.RatingCount -= 1;
        var totalRatingValue = existedRating.Model.Ratings.Aggregate(0, (total, rating) => total + rating.Value) - existedRating.Value;
        existedRating.Model.RatingAverage = totalRatingValue / existedRating.Model.RatingCount;

        // update model rating values
        existedRating.Model.Ratings = null;
        _dbCommandService.Update<Model>(existedRating.Model);

        // Remove existedRating
        _dbCommandService.Remove<Rating>(existedRating);

        // save both updated model values and existed rating to db
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(new { Message = $"Rating Record with CustomerId: {customerId} and ModelId: {command.ModelId} was Removed Successfully ..." });
    }
}
