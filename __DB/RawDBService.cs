using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

using TechStoreApi.DB.Common;

namespace TechStoreApi.DB;

public interface IRawDBService
{
    #region Query [Select]
    Task<T> FindAsync<T>(Guid id) where T : class;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where) where T : class;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : class;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : class;

    Task<List<T>> GetAllAsync<T>() where T : class;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : class;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : class;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : class;
    Task<PageResult<T>> GetPageAsync<T>(IQueryable<T> query, int pageSize, int pageNumber) where T : class;
    Task<PageResult<dynamic>> GetPageAsync(IQueryable query, int pageSize = 10, int pageNumber = 1);

    Task<int> CountAsync<T>() where T : class;
    Task<int> CountAsync<T>(Expression<Func<T, bool>> where) where T : class;

    IQueryable<T> GetQuery<T>() where T : class;
    IQueryable<T> GetQueryWithTracking<T>() where T : class;

    #endregion

    #region Commands [Add-Update-Delete]
    void Add<T>(T item, string createdBy) where T : class;
    void AddRange<T>(List<T> range, string createdBy) where T : class;

    void Update<T>(T item, string modifiedBy) where T : class;
    void UpdateRange<T>(List<T> range, string modifiedBy) where T : class;

    void Remove<T>(T item) where T : class;
    void RemoveRange<T>(List<T> range) where T : class;

    Task<bool> GetOneAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : class;
    Task<bool> GetListAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : class;

    Task<int> ExecuteUpdateAsync<T>(Expression<Func<T, bool>> where, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setter) where T : class;
    Task<int> ExecuteDeleteAsync<T>(Expression<Func<T, bool>> where) where T : class;

    #endregion

    #region [UnitOfEWork]
    void Attach<T>(T entity) where T : class;
    void SetState<T>(T entity, string state) where T : class;
    Task<int> SaveChangesAsync();

    #endregion
}

public class RawDBService : IRawDBService
{
    private readonly TechStoreDB _db;
    public RawDBService(TechStoreDB db) => _db = db;

    #region Queries [Select]
    public async Task<T> FindAsync<T>(Guid id) where T : class
    {
        return await _db.Set<T>().FindAsync(id);
    }
    public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        return await _db.Set<T>()
            .AsNoTracking()
            .Where(where)
            .FirstOrDefaultAsync();
    }
    public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : class
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.Where(where).FirstOrDefaultAsync();
    }
    public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : class
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.Where(where).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        return await _db.Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        return await _db.Set<T>()
            .AsNoTracking()
            .Where(where)
            .ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : class
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query
            .Where(where)
            .ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : class
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query
            .Where(where)
            .ToListAsync();
    }

    public async Task<PageResult<T>> GetPageAsync<T>(IQueryable<T> query, int pageSize = 10, int pageNumber = 1) where T : class
    {
        var count = await query.CountAsync();
        return new PageResult<T>
        {
            PageItems = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync(),
            TotalItems = count,
            TotalPages = (int) Math.Ceiling((decimal) count / pageSize),
        };
    }

    public async Task<PageResult<dynamic>> GetPageAsync(IQueryable query, int pageSize = 10, int pageNumber = 1)
    {
        var count = query.Count();
        return new PageResult<dynamic>
        {
            PageItems = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToDynamicListAsync(),
            TotalItems = count,
            TotalPages = (int) Math.Ceiling((decimal) count / pageSize),
        };
    }

    public async Task<int> CountAsync<T>() where T : class
    {
        return await _db.Set<T>().CountAsync();
    }
    public async Task<int> CountAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        return await _db.Set<T>().CountAsync(where);
    }

    public IQueryable<T> GetQuery<T>() where T : class
    {
        return _db.Set<T>().AsNoTracking();
    }
    public IQueryable<T> GetQueryWithTracking<T>() where T : class
    {
        return _db.Set<T>();
    }

    #endregion

    #region Commands [Add-Update-Delete]
    public void Add<T>(T item, string createdBy = "app_dev") where T : class
    {
        _db.Set<T>().Add(item);
    }
    public void AddRange<T>(List<T> range, string createdBy = "app_dev") where T : class
    {
        _db.Set<T>().AddRange(range);
    }

    public void Update<T>(T item, string modifiedBy = "app_dev") where T : class
    {
        _db.Set<T>().Update(item);
    }
    public void UpdateRange<T>(List<T> range, string modifiedBy = "app_dev") where T : class
    {
        _db.Set<T>().UpdateRange(range);
    }

    public void Remove<T>(T item) where T : class
    {
        _db.Set<T>().Remove(item);
    }
    public void RemoveRange<T>(List<T> range) where T : class
    {
        _db.Set<T>().RemoveRange(range);
    }

    public async Task<bool> GetOneAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        var set = _db.Set<T>();
        T item = await set.Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            set.Remove(item);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        var set = _db.Set<T>();
        var items = await set.Where(where).ToListAsync();
        if (items.Count != 0)
        {
            set.RemoveRange(items);
            return true;
        }
        return false;
    }

    public async Task<int> ExecuteUpdateAsync<T>(Expression<Func<T, bool>> where, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setter) where T : class
    {
        return await _db.Set<T>().Where(where).ExecuteUpdateAsync(setter);
    }
    public async Task<int> ExecuteDeleteAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        return await _db.Set<T>().Where(where).ExecuteDeleteAsync();
    }

    #endregion

    #region UnitOfWork
    public void Attach<T>(T entity) where T : class
    {
        _db.Set<T>().Attach(entity);
    }
    public void SetState<T>(T entity, string state) where T : class
    {
        switch (state)
        {
            case "Added":
                _db.Entry(entity).State = EntityState.Added;
                break;
            case "Modified":
                _db.Entry(entity).State = EntityState.Modified;
                break;
            case "Deleted":
                _db.Entry(entity).State = EntityState.Deleted;
                break;
            default:
                _db.Entry(entity).State = EntityState.Unchanged;
                break;
        }
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }

    #endregion
}
