using MediatR;
using Microsoft.AspNetCore.Mvc;

using Brands.Queries;
using Brands.Commands;

namespace Brands;

[ApiController]
[Route("api/[controller]/[action]")]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;
    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// listType values (all/deleted/existed)
    /// Brands/List?type=existed
    /// </summary>
    /// <returns>List of BrandDto</returns>
    [HttpGet]
    public Task<IResult> List(int? pageSize, int? pageNumber, string type = "existed")
    {
        return _mediator.Send(new ListBrandsQuery() { ListType = type, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Brands/Search?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of BrandDto</returns>
    [HttpGet]
    public Task<IResult> Search(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchBrandsQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Brands/FindOne/[id]
    /// </summary>
    /// <returns>BrandDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> FindOne(Guid id)
    {
        return _mediator.Send(new FindBrandQuery() { Id = id });
    }
    /// <summary>
    /// Brands/FindList/[ids]
    /// </summary>
    /// <returns>List of BrandDto</returns>
    [HttpGet("{ids}")]
    public Task<IResult> FindList(string ids)
    {
        return _mediator.Send(new FindBrandsQuery() { Ids = ids });
    }

    /// <summary>
    /// Brands/Add
    /// </summary>
    /// <returns>BrandDto</returns>
    [HttpPost]
    public Task<IResult> Add(AddBrandCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Brands/AddMany
    /// </summary>
    /// <returns>List of BrandDto</returns>
    [HttpPost]
    public Task<IResult> AddMany(AddManyBrandsCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Brands/Update
    /// </summary>
    /// <returns>BrandDto</returns>
    [HttpPut]
    public Task<IResult> Update(UpdateBrandCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Brands/UpdateMany
    /// </summary>
    /// <returns>List of BrandDto</returns>
    [HttpPut]
    public Task<IResult> UpdateMany(UpdateManyBrandsCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Brands/Delete
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Delete(DeleteBrandCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Brands/Restore
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Restore(RestoreBrandCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Brands/Activate
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Activate(ActivateBrandCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Brands/Disable
    /// </summary>
    /// <returns>bool</returns>
    [HttpPut]
    public Task<IResult> Disable(DisableBrandCommand command)
    {
        return _mediator.Send(command);
    }

}
