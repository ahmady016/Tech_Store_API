using Microsoft.AspNetCore.Mvc;
using MediatR;

using Favorites.Queries;
using Favorites.Commands;

namespace Favorites;

[ApiController]
[Route("api/[controller]/[action]")]
public class FavoritesController : ControllerBase
{
    private readonly IMediator _mediator;
    public FavoritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Favorites/ListAll/
    /// </summary>
    /// <returns>List of CustomerFavoriteModelDto</returns>
    [HttpGet]
    public Task<IResult> ListAll(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListAllFavoritesQuery() { PageNumber = pageNumber, PageSize = pageSize });
    }
    /// <summary>
    /// Favorites/ListCustomerFavoriteModels/[customerId]
    /// </summary>
    /// <returns>List of CustomerFavoriteModelDto</returns>
    [HttpGet("{customerId}")]
    public Task<IResult> ListCustomerFavoriteModels(string customerId, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListCustomerFavoriteModelsQuery() { CustomerId = customerId, PageNumber = pageNumber, PageSize = pageSize });
    }
    /// <summary>
    /// Favorites/ListModelFavoriteCustomers/[modelId]
    /// </summary>
    /// <returns>List of CustomerFavoriteModelDto</returns>
    [HttpGet("{modelId}")]
    public Task<IResult> ListModelFavoriteCustomers(Guid modelId, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListModelFavoriteCustomersQuery() { ModelId = modelId, PageNumber = pageNumber, PageSize = pageSize });
    }

    /// <summary>
    /// Favorites/AddModelToFavorite
    /// </summary>
    /// <returns>CustomerFavoriteModelDto</returns>
    [HttpPost]
    public Task<IResult> AddModelToFavorite(AddModelToFavoriteCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Favorites/UpdateCustomerFavoriteModels
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> UpdateCustomerFavoriteModels(UpdateCustomerFavoriteModelsCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Favorites/RemoveModelFromFavorite
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> RemoveModelFromFavorite(RemoveModelFromFavoriteCommand command)
    {
        return _mediator.Send(command);
    }

}
