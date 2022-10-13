using System.Net;
using AutoMapper;
using System.Linq.Dynamic.Core;

using Common;
using DB.Common;

namespace DB;

public interface ICrudService
{
    Task<T> GetByIdAsync<T>(Guid id) where T : Entity;
    Task<List<T>> GetByIdsAsync<T>(List<Guid> ids) where T : Entity;

    Task<List<TDto>> ListAsync<T, TDto>(string type = "existed") where T : Entity;
    Task<PageResult<TDto>> ListPageAsync<T, TDto>(string type = "existed", int pageSize = 10, int pageNumber = 1) where T : Entity;

    Task<IResult> QueryAsync<T, TDto>(string where, string select, string orderBy) where T : Entity;
    Task<IResult> QueryPageAsync<T, TDto>(string where, string select, string orderBy, int pageSize = 10, int pageNumber = 1) where T : Entity;

    Task<TDto> FindAsync<T, TDto>(Guid id) where T : Entity;
    Task<List<TDto>> FindListAsync<T, TDto>(string ids) where T : Entity;

    Task<TDto> AddAsync<T, TDto, TCreateInput>(TCreateInput input) where T : Entity;
    Task<List<TDto>> AddManyAsync<T, TDto, TCreateInput>(List<TCreateInput> inputs) where T : Entity;

    Task<TDto> UpdateAsync<T, TDto, TUpdate, TCommand>(TUpdate input, T oldItem = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class;
    Task<List<TDto>> UpdateManyAsync<T, TDto, TUpdate, TCommand>(List<TUpdate> inputs, List<T> oldItems = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class;

    Task<bool> DeleteAsync<T>(Guid id) where T : Entity;
    Task<bool> RestoreAsync<T>(Guid id) where T : Entity;

    Task<bool> ActivateAsync<T>(Guid id) where T : Entity;
    Task<bool> DisableAsync<T>(Guid id) where T : Entity;
}

public class CrudService : ICrudService
{
    private readonly IDBService _dbService;
    private readonly IMapper _mapper;
    private readonly ILogger<Entity> _logger;
    private string _errorMessage;

    public CrudService(
        IDBService dbService,
        IMapper mapper,
        ILogger<Entity> logger
    )
    {
        _dbService = dbService;
        _mapper = mapper;
        _logger = logger;
    }

    private static void FillNonInputValues<T>(T oldItem, T newItem) where T : Entity
    {
        newItem.Id = oldItem.Id;
        newItem.CreatedAt = oldItem.CreatedAt;
        newItem.CreatedBy = oldItem.CreatedBy;

        newItem.ModifiedAt = oldItem.ModifiedAt;
        newItem.ModifiedBy = oldItem.ModifiedBy;

        newItem.IsActive = oldItem.IsActive;
        newItem.ActivatedAt = oldItem.ActivatedAt;
        newItem.ActivatedBy = oldItem.ActivatedBy;
        newItem.DisabledAt = oldItem.DisabledAt;
        newItem.DisabledBy = oldItem.DisabledBy;

        newItem.IsDeleted = oldItem.IsDeleted;
        newItem.DeletedAt = oldItem.DeletedAt;
        newItem.DeletedBy = oldItem.DeletedBy;
        newItem.RestoredAt = oldItem.RestoredAt;
        newItem.RestoredBy = oldItem.RestoredBy;
    }
    public async Task<T> GetByIdAsync<T>(Guid id) where T : Entity
    {
        var dbItem = await _dbService.FindAsync<T>(id);
        if (dbItem is null)
        {
            _errorMessage = $"{typeof(T).Name} Record with Id: {id} is Not Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }
        return dbItem;
    }
    public async Task<List<T>> GetByIdsAsync<T>(List<Guid> ids) where T : Entity
    {
        var list = await _dbService.GetListAsync<T>(e => ids.Contains(e.Id));
        if (list.Count == 0)
        {
            _errorMessage = $"No Any {typeof(T).Name} Records Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }
        return list;
    }

    public async Task<List<TDto>> ListAsync<T, TDto>(string type = "existed") where T : Entity
    {
        var list = type.ToLower() switch
        {
            "all" => await _dbService.GetAllAsync<T>(),
            "deleted" => await _dbService.GetListAsync<T>(e => e.IsDeleted),
            _ => await _dbService.GetListAsync<T>(e => !e.IsDeleted),
        };

        return _mapper.Map<List<TDto>>(list);
    }
    public async Task<PageResult<TDto>> ListPageAsync<T, TDto>(string type = "existed", int pageSize = 10, int pageNumber = 1) where T : Entity
    {
        var query = type.ToLower() switch
        {
            "all" => _dbService.GetQuery<T>(),
            "deleted" => _dbService.GetQuery<T>(e => e.IsDeleted),
            _ => _dbService.GetQuery<T>(e => !e.IsDeleted),
        };

        var page = await _dbService.GetPageAsync(query, pageSize, pageNumber);
        return new PageResult<TDto>()
        {
            PageItems = _mapper.Map<List<TDto>>(page.PageItems),
            TotalItems = page.TotalItems,
            TotalPages = page.TotalPages
        };
    }

    public async Task<IResult> QueryAsync<T, TDto>(string where, string select, string orderBy) where T : Entity
    {
        if (where is null && select is null && orderBy is null)
        {
            _errorMessage = $"{typeof(T).Name}: Must supply at least one of the following: [where, select, orderBy]";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }

        var query = _dbService.GetQuery<T>();
        if (where is not null)
            query = query.Where(where);
        if (orderBy is not null)
            query = query.OrderBy(orderBy.RemoveEmptyElements(','));
        if (select is not null)
            query = query.Select(select.RemoveEmptyElements(',')) as IQueryable<T>;

        if(select is not null)
            return await query.ToDynamicListAsync() as IResult;
        return _mapper.Map<List<TDto>>(await query.ToDynamicListAsync()) as IResult;
    }
    public async Task<IResult> QueryPageAsync<T, TDto>(string where, string select, string orderBy, int pageSize = 10, int pageNumber = 1) where T : Entity
    {
        if (where is null && select is null && orderBy is null)
        {
            _errorMessage = $"{typeof(T).Name}: Must supply at least one of the following: [where, select, orderBy]";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }

        var query = _dbService.GetQuery<T>();
        if (where is not null)
            query = query.Where(where);
        if (orderBy is not null)
            query = query.OrderBy(orderBy.RemoveEmptyElements(','));
        if (select is not null)
            query = query.Select(select.RemoveEmptyElements(',')) as IQueryable<T>;

        var page = await _dbService.GetPageAsync<T>(query, pageSize, pageNumber);
        if(select is not null)
            return page as IResult;
        return new PageResult<TDto>()
        {
            PageItems = _mapper.Map<List<TDto>>(page.PageItems),
            TotalItems = page.TotalItems,
            TotalPages = page.TotalPages
        } as IResult;
    }

    public async Task<TDto> FindAsync<T, TDto>(Guid id) where T : Entity
    {
        var dbItem = await GetByIdAsync<T>(id);
        return _mapper.Map<TDto>(dbItem);
    }
    public async Task<List<TDto>> FindListAsync<T, TDto>(string ids) where T : Entity
    {
        if (ids == null)
        {
            _errorMessage = $"{typeof(T).Name}: Must supply comma separated string of ids";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.BadRequest);
        }
        var _ids = ids.SplitAndRemoveEmpty(',').Select(Guid.Parse).ToList();
        var list = await GetByIdsAsync<T>(_ids);
        return _mapper.Map<List<TDto>>(list);
    }

    public async Task<TDto> AddAsync<T, TDto, TCreateInput>(TCreateInput input) where T : Entity
    {
        var dbItem = _mapper.Map<T>(input);
        var createdItem = _dbService.Add<T>(dbItem);
        await _dbService.SaveChangesAsync();
        return _mapper.Map<TDto>(createdItem);
    }
    public async Task<List<TDto>> AddManyAsync<T, TDto, TCreateInput>(List<TCreateInput> inputs) where T : Entity
    {
        var dbItems = _mapper.Map<List<T>>(inputs);
        var createdItems = _dbService.AddAndGetRange<T>(dbItems);
        await _dbService.SaveChangesAsync();
        return _mapper.Map<List<TDto>>(createdItems);
    }

    public async Task<TDto> UpdateAsync<T, TDto, TUpdate, TCommand>(TUpdate input, T oldItem = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class
    {
        if(oldItem is null)
            oldItem = await GetByIdAsync<T>(input.Id);

        var newItem = _mapper.Map<T>(input.ModifiedEntity);
        FillNonInputValues(oldItem, newItem);

        var updatedItem = _dbService.Update<T>(newItem);
        await _dbService.SaveChangesAsync();

        return _mapper.Map<TDto>(updatedItem);
    }
    public async Task<List<TDto>> UpdateManyAsync<T, TDto, TUpdate, TCommand>(List<TUpdate> inputs, List<T> oldItems = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class
    {
        if(oldItems is null)
            oldItems = await GetByIdsAsync<T>(inputs.Select(x => x.Id).ToList());
        var newItems = _mapper.Map<List<T>>(inputs.Select(x => x.ModifiedEntity));

        for (int i = 0; i < oldItems.Count; i++)
            FillNonInputValues(oldItems[i], newItems[i]);

        var updatedItems = _dbService.UpdateAndGetRange<T>(newItems);
        await _dbService.SaveChangesAsync();

        return _mapper.Map<List<TDto>>(updatedItems);
    }

    public async Task<bool> DeleteAsync<T>(Guid id) where T : Entity
    {
        var dbItem = await GetByIdAsync<T>(id);
        _dbService.Delete<T>(dbItem);
        await _dbService.SaveChangesAsync();
        return true;
    }
    public async Task<bool> RestoreAsync<T>(Guid id) where T : Entity
    {
        var dbItem = await GetByIdAsync<T>(id);
        _dbService.Restore<T>(dbItem);
        await _dbService.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateAsync<T>(Guid id) where T : Entity
    {
        var dbItem = await GetByIdAsync<T>(id);
        _dbService.Activate<T>(dbItem);
        await _dbService.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DisableAsync<T>(Guid id) where T : Entity
    {
        var dbItem = await GetByIdAsync<T>(id);
        _dbService.Disable<T>(dbItem);
        await _dbService.SaveChangesAsync();
        return true;
    }

}
