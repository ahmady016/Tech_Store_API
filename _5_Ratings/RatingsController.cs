using Microsoft.AspNetCore.Mvc;
using MediatR;

using Ratings.Queries;
using Ratings.Commands;

namespace Ratings;

[ApiController]
[Route("api/[controller]/[action]")]
public class RatingsController : ControllerBase
{
    private readonly IMediator _mediator;
    public RatingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Ratings/ListAll/
    /// </summary>
    /// <returns>List of RatingDto</returns>
    [HttpGet]
    public Task<IResult> ListAll(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListAllRatingsQuery() { PageNumber = pageNumber, PageSize = pageSize });
    }
    /// <summary>
    /// Ratings/ListAllCustomerRatings/[customerId]
    /// </summary>
    /// <returns>List of RatingDto</returns>
    [HttpGet("{customerId}")]
    public Task<IResult> ListAllCustomerRatings(string customerId, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListAllCustomerRatingsQuery() { CustomerId = customerId, PageNumber = pageNumber, PageSize = pageSize });
    }
    /// <summary>
    /// Ratings/ListAllModelRatings/[modelId]
    /// </summary>
    /// <returns>List of RatingDto</returns>
    [HttpGet("{modelId}")]
    public Task<IResult> ListAllModelRatings(Guid modelId, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListAllModelRatingsQuery() { ModelId = modelId, PageNumber = pageNumber, PageSize = pageSize });
    }

    /// <summary>
    /// Ratings/Add
    /// </summary>
    /// <returns>RatingDto</returns>
    [HttpPost]
    public Task<IResult> Add(AddRatingCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Ratings/Update
    /// </summary>
    /// <returns>RatingDto</returns>
    [HttpPut]
    public Task<IResult> Update(UpdateRatingCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Ratings/Remove
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> Remove(RemoveRatingCommand command)
    {
        return _mediator.Send(command);
    }

}
