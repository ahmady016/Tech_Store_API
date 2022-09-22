using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using DB.Common;

namespace DB;

public interface IDBQueryService
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
}

public class DBQueryService : IDBQueryService
{
    private readonly TechStoreDB _db;
    public DBQueryService(TechStoreDB db)
    {
        _db = db;
    }

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
            PageItems = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync(),
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

}
