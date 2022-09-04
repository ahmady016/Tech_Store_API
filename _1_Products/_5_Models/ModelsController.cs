using Microsoft.AspNetCore.Mvc;
using MediatR;

using Models.Queries;
using Models.Commands;

namespace Models;

[ApiController]
[Route("api/[controller]/[action]")]
public class ModelsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ModelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// listType values (all/deleted/existed)
    /// Models/List?type=existed
    /// </summary>
    /// <returns>List of ModelDto</returns>
    [HttpGet]
    public Task<IResult> List(int? pageSize, int? pageNumber, string type = "existed")
    {
        return _mediator.Send(new ListModelsQuery() { ListType = type, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Models/Search?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of ModelDto</returns>
    [HttpGet]
    public Task<IResult> Search(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchModelsQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Models/Find/[id]
    /// </summary>
    /// <returns>ModelDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> FindOne(Guid id)
    {
        return _mediator.Send(new FindModelQuery() { Id = id });
    }
    /// <summary>
    /// Models/Find/[ids]
    /// </summary>
    /// <returns>List of ModelDto</returns>
    [HttpGet("{ids}")]
    public Task<IResult> FindList(string ids)
    {
        return _mediator.Send(new FindModelsQuery() { Ids = ids });
    }

    /// <summary>
    /// Models/Add
    /// </summary>
    /// <returns>ModelDto</returns>
    [HttpPost]
    public Task<IResult> Add(AddModelCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Models/AddMany
    /// </summary>
    /// <returns>List of ModelDto</returns>
    [HttpPost]
    public Task<IResult> AddMany(AddManyModelsCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Models/Update
    /// </summary>
    /// <returns>ModelDto</returns>
    [HttpPut]
    public Task<IResult> Update(UpdateModelCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Models/UpdateMany
    /// </summary>
    /// <returns>List of ModelDto</returns>
    [HttpPut]
    public Task<IResult> UpdateMany(UpdateManyModelsCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Models/Delete
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Delete(DeleteModelCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Models/Restore
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Restore(RestoreModelCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Models/Activate
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Activate(ActivateModelCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Models/Disable
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Disable(DisableModelCommand command)
    {
        return _mediator.Send(command);
    }

}
