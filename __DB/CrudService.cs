using System.Net;
using AutoMapper;
using System.Linq.Dynamic.Core;

using Common;
using DB.Common;

namespace DB;

public interface ICrudService
{
    T GetById<T>(Guid id) where T : Entity;
    List<T> GetByIds<T>(List<Guid> ids) where T : Entity;
    List<TDto> List<T, TDto>(string type = "existed") where T : Entity;
    PageResult<TDto> ListPage<T, TDto>(string type = "existed", int pageSize = 10, int pageNumber = 1) where T : Entity;

    IResult Query<T, TDto>(string where, string select, string orderBy) where T : Entity;
    IResult QueryPage<T, TDto>(string where, string select, string orderBy, int pageSize = 10, int pageNumber = 1) where T : Entity;

    TDto Find<T, TDto>(Guid id) where T : Entity;
    List<TDto> FindList<T, TDto>(string ids) where T : Entity;

    TDto Add<T, TDto, TCreateInput>(TCreateInput input) where T : Entity;
    List<TDto> AddMany<T, TDto, TCreateInput>(List<TCreateInput> inputs) where T : Entity;

    TDto Update<T, TDto, TUpdate, TCommand>(TUpdate input, T oldItem = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class;
    List<TDto> UpdateMany<T, TDto, TUpdate, TCommand>(List<TUpdate> inputs, List<T> oldItems = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class;

    bool Delete<T>(Guid id) where T : Entity;
    bool Restore<T>(Guid id) where T : Entity;

    bool Activate<T>(Guid id) where T : Entity;
    bool Disable<T>(Guid id) where T : Entity;
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
    public T GetById<T>(Guid id) where T : Entity
    {
        var dbItem = _dbService.Find<T>(id);
        if (dbItem is null)
        {
            _errorMessage = $"{typeof(T).Name} Record with Id: {id} is Not Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }
        return dbItem;
    }
    public List<T> GetByIds<T>(List<Guid> ids) where T : Entity
    {
        var list = _dbService.GetList<T>(e => ids.Contains(e.Id));
        if (list.Count == 0)
        {
            _errorMessage = $"No Any {typeof(T).Name} Records Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }
        return list;
    }

    public List<TDto> List<T, TDto>(string type = "existed") where T : Entity
    {
        var list = type.ToLower() switch
        {
            "all" => _dbService.GetAll<T>(),
            "deleted" => _dbService.GetList<T>(e => e.IsDeleted),
            _ => _dbService.GetList<T>(e => !e.IsDeleted),
        };

        return _mapper.Map<List<TDto>>(list);
    }
    public PageResult<TDto> ListPage<T, TDto>(string type = "existed", int pageSize = 10, int pageNumber = 1) where T : Entity
    {
        var query = type.ToLower() switch
        {
            "all" => _dbService.GetQuery<T>(),
            "deleted" => _dbService.GetQuery<T>(e => e.IsDeleted),
            _ => _dbService.GetQuery<T>(e => !e.IsDeleted),
        };

        var page = _dbService.GetPage(query, pageSize, pageNumber);
        return new PageResult<TDto>()
        {
            PageItems = _mapper.Map<List<TDto>>(page.PageItems),
            TotalItems = page.TotalItems,
            TotalPages = page.TotalPages
        };
    }

    public IResult Query<T, TDto>(string where, string select, string orderBy) where T : Entity
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
            return query.ToList() as IResult;
        return _mapper.Map<List<TDto>>(query.ToList()) as IResult;
    }
    public IResult QueryPage<T, TDto>(string where, string select, string orderBy, int pageSize = 10, int pageNumber = 1) where T : Entity
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

        var page = _dbService.GetPage<T>(query, pageSize, pageNumber);
        if(select is not null)
            return page as IResult;
        return new PageResult<TDto>()
        {
            PageItems = _mapper.Map<List<TDto>>(page.PageItems),
            TotalItems = page.TotalItems,
            TotalPages = page.TotalPages
        } as IResult;
    }

    public TDto Find<T, TDto>(Guid id) where T : Entity
    {
        var dbItem = GetById<T>(id);
        return _mapper.Map<TDto>(dbItem);
    }
    public List<TDto> FindList<T, TDto>(string ids) where T : Entity
    {
        if (ids == null)
        {
            _errorMessage = $"{typeof(T).Name}: Must supply comma separated string of ids";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.BadRequest);
        }
        var _ids = ids.SplitAndRemoveEmpty(',').Select(Guid.Parse).ToList();
        var list = GetByIds<T>(_ids);
        return _mapper.Map<List<TDto>>(list);
    }

    public TDto Add<T, TDto, TCreateInput>(TCreateInput input) where T : Entity
    {
        var dbItem = _mapper.Map<T>(input);
        var createdItem = _dbService.Add<T>(dbItem);
        _dbService.SaveChanges();
        return _mapper.Map<TDto>(createdItem);
    }
    public List<TDto> AddMany<T, TDto, TCreateInput>(List<TCreateInput> inputs) where T : Entity
    {
        var dbItems = _mapper.Map<List<T>>(inputs);
        var createdItems = _dbService.AddAndGetRange<T>(dbItems);
        _dbService.SaveChanges();
        return _mapper.Map<List<TDto>>(createdItems);
    }

    public TDto Update<T, TDto, TUpdate, TCommand>(TUpdate input, T oldItem = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class
    {
        if(oldItem is null)
            oldItem = GetById<T>(input.Id);

        var newItem = _mapper.Map<T>(input.ModifiedEntity);
        FillNonInputValues(oldItem, newItem);

        var updatedItem = _dbService.Update<T>(newItem);
        _dbService.SaveChanges();

        return _mapper.Map<TDto>(updatedItem);
    }
    public List<TDto> UpdateMany<T, TDto, TUpdate, TCommand>(List<TUpdate> inputs, List<T> oldItems = null) where T : Entity where TUpdate : UpdateCommand<TCommand> where TCommand : class
    {
        if(oldItems is null)
            oldItems = GetByIds<T>(inputs.Select(x => x.Id).ToList());
        var newItems = _mapper.Map<List<T>>(inputs.Select(x => x.ModifiedEntity));

        for (int i = 0; i < oldItems.Count; i++)
            FillNonInputValues(oldItems[i], newItems[i]);

        var updatedItems = _dbService.UpdateAndGetRange<T>(newItems);
        _dbService.SaveChanges();

        return _mapper.Map<List<TDto>>(updatedItems);
    }

    public bool Delete<T>(Guid id) where T : Entity
    {
        var dbItem = GetById<T>(id);
        _dbService.Delete<T>(dbItem);
        _dbService.SaveChanges();
        return true;
    }
    public bool Restore<T>(Guid id) where T : Entity
    {
        var dbItem = GetById<T>(id);
        _dbService.Restore<T>(dbItem);
        _dbService.SaveChanges();
        return true;
    }

    public bool Activate<T>(Guid id) where T : Entity
    {
        var dbItem = GetById<T>(id);
        _dbService.Activate<T>(dbItem);
        _dbService.SaveChanges();
        return true;
    }
    public bool Disable<T>(Guid id) where T : Entity
    {
        var dbItem = GetById<T>(id);
        _dbService.Disable<T>(dbItem);
        _dbService.SaveChanges();
        return true;
    }

}
