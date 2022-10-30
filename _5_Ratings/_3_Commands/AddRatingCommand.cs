using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Entities;
using Dtos;
using Auth;

namespace Ratings.Commands;

public class AddRatingCommand : RatingInput {}

public class AddRatingCommandHandler : IRequestHandler<AddRatingCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Rating> _logger;
    private string _errorMessage;
    public AddRatingCommandHandler(
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
        AddRatingCommand command,
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

        // calc the ratingsCount and ratingAverage
        existedModel.RatingCount += 1;
        var totalRatingValue = existedModel.Ratings.Aggregate(0, (total, rating) => total + rating.Value) + command.Value;
        existedModel.RatingAverage = totalRatingValue / existedModel.RatingCount;

        // update model rating values
        existedModel.Ratings = null;
        _dbCommandService.Update<Model>(existedModel);

        // add newRating
        var newRating = _mapper.Map<Rating>(command);
        newRating.CustomerId = existedCustomer.Id;
        _dbCommandService.Add<Rating>(newRating);

        // save both updated model values and new rating to db
        await _dbCommandService.SaveChangesAsync();

        // fill in missing props and get ratingDto
        newRating.Customer = existedCustomer;
        newRating.Model = existedModel;
        var ratingDto = _mapper.Map<Rating>(newRating);

        return Results.Ok(ratingDto);
    }
}
