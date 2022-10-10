using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Entities;

namespace Ratings.Commands;

public class RemoveRatingCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "CustomerId is Required")]
    [StringLength(450, MinimumLength = 36, ErrorMessage = "CustomerId must between 36 and 450 characters")]
    public string CustomerId { get; set; }

    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
}

public class RemoveRatingCommandHandler : IRequestHandler<RemoveRatingCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Rating> _logger;
    private string _errorMessage;
    public RemoveRatingCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Rating> logger
    )
    {
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
        var existedRating = await _dbQueryService
            .GetQuery<Rating>(e => e.CustomerId == command.CustomerId && e.ModelId == command.ModelId)
            .Include("Model")
            .Include("Model.Ratings")
            .FirstOrDefaultAsync();
        if(existedRating is null)
        {
            _errorMessage = $"Rating Record with CustomerId: {command.CustomerId} and ModelId: {command.ModelId} Not Found";
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

        return Results.Ok(new { Message = $"Rating Record with CustomerId: {command.CustomerId} and ModelId: {command.ModelId} was Removed Successfully ..." });
    }
}
