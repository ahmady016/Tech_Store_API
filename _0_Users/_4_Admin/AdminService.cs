
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using DB;
using DB.Common;

namespace Admin;

public interface IAdminService
{
    Task<T> FindAsync<T>(string id) where T : class;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where) where T : class;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : class;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : class;

    Task<List<T>> GetAllAsync<T>() where T : class;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : class;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : class;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : class;

    IQueryable<T> GetQuery<T>() where T : class;
    IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> where) where T : class;
    Task<PageResult<T>> GetPageAsync<T>(IQueryable<T> query, int pageSize, int pageNumber) where T : class;

    Task<int> CountAsync<T>() where T : class;
    Task<int> CountAsync<T>(Expression<Func<T, bool>> where) where T : class;

    void Add<T>(T item) where T : class;
    void AddRange<T>(List<T> range) where T : class;
    void Update<T>(T item) where T : class;
    void UpdateRange<T>(List<T> range) where T : class;
    void Remove<T>(T item) where T : class;
    void RemoveRange<T>(List<T> items) where T : class;
    Task<bool> GetOneAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : class;
    Task<bool> GetListAndRemoveRangeAsync<T>(Expression<Func<T, bool>> where) where T : class;

    void Attach<T>(T entity) where T : Entity;
    void SetState<T>(T entity, string state) where T : Entity;
    Task<int> SaveChangesAsync();
}

public class AdminService : IAdminService
{
    private readonly TechStoreDB _db;
    public AdminService(TechStoreDB db)
    {
        _db = db;
    }

    #region Queries
    public async Task<T> FindAsync<T>(string id) where T : class
    {
        var entity = await _db.Set<T>().FindAsync(id);
        if(entity is not null)
            _db.Entry(entity).State = EntityState.Detached;
        return entity;
    }
    public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        return await _db.Set<T>().AsNoTracking().Where(where).FirstOrDefaultAsync();
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
        return await _db.Set<T>().AsNoTracking().ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        return await _db.Set<T>().AsNoTracking().Where(where).ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : class
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.Where(where).ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : class
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.Where(where).ToListAsync();
    }

    public IQueryable<T> GetQuery<T>() where T : class
    {
        return _db.Set<T>().AsNoTracking();
    }
    public IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> where) where T : class
    {
        return _db.Set<T>().AsNoTracking().Where(where);
    }
    public async Task<PageResult<T>> GetPageAsync<T>(IQueryable<T> query, int pageSize, int pageNumber) where T : class
    {
        var count = await query.CountAsync();
        return new PageResult<T>
        {
            PageItems = query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList(),
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
    #endregion

    #region Commands
    public void Add<T>(T item) where T : class
    {
        _db.Set<T>().Add(item);
    }
    public void AddRange<T>(List<T> range) where T : class
    {
        _db.Set<T>().AddRange(range);
    }
    public void Update<T>(T item) where T : class
    {
        _db.Set<T>().Update(item);
    }
    public void UpdateRange<T>(List<T> range) where T : class
    {
        _db.Set<T>().UpdateRange(range);
    }
    public void Remove<T>(T item) where T : class
    {
        _db.Set<T>().Remove(item);
    }
    public void RemoveRange<T>(List<T> items) where T : class
    {
        _db.Set<T>().RemoveRange(items);
    }
    public async Task<bool> GetOneAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        var set = _db.Set<T>();
        var item = await set.Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            set.Remove(item);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndRemoveRangeAsync<T>(Expression<Func<T, bool>> where) where T : class
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
    #endregion

    #region UnitOfWork
    public void Attach<T>(T entity) where T : Entity
    {
        _db.Set<T>().Attach(entity);
    }
    public void SetState<T>(T entity, string state) where T : Entity
    {
        _db.Entry(entity).State = state switch
        {
            "Added" =>     EntityState.Added,
            "Modified" =>  EntityState.Modified,
            "Deleted" =>   EntityState.Deleted,
            _ =>           EntityState.Unchanged
        };
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }
    #endregion

}
