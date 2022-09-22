using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using DB.Common;

namespace DB;

public interface IDBCommandService
{
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

public class DBCommandService : IDBCommandService
{
    private readonly TechStoreDB _db;
    public DBCommandService(TechStoreDB db)
    {
        _db = db;
    }

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

}
