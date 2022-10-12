using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using DB.Common;

namespace DB;

public interface IDBService
{
    #region Query [Select]
    T Find<T>(Guid id) where T : Entity;
    T GetOne<T>(Expression<Func<T, bool>> where) where T : Entity;
    T GetOne<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity;
    T GetOne<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity;

    List<T> GetAll<T>() where T : Entity;
    List<T> GetList<T>(Expression<Func<T, bool>> where) where T : Entity;
    List<T> GetList<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity;
    List<T> GetList<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity;

    IQueryable<T> GetQuery<T>() where T : Entity;
    IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> where) where T : Entity;
    PageResult<T> GetPage<T>(IQueryable<T> query, int pageSize, int pageNumber) where T : Entity;

    int Count<T>() where T : Entity;
    int Count<T>(Expression<Func<T, bool>> where) where T : Entity;
    #endregion

    #region Commands [Add-Update-Delete]
    T Add<T>(T item) where T : Entity;
    void AddRange<T>(List<T> range) where T : Entity;
    List<T> AddAndGetRange<T>(List<T> range) where T : Entity;

    T Update<T>(T item) where T : Entity;
    void UpdateRange<T>(List<T> range) where T : Entity;
    List<T> UpdateAndGetRange<T>(List<T> range) where T : Entity;

    void Activate<T>(T item) where T : Entity;
    void ActivateRange<T>(List<T> range) where T : Entity;

    void Disable<T>(T item) where T : Entity;
    void DisableRange<T>(List<T> range) where T : Entity;

    void Delete<T>(T item) where T : Entity;
    void DeleteRange<T>(List<T> range) where T : Entity;

    void Restore<T>(T item) where T : Entity;
    void RestoreRange<T>(List<T> range) where T : Entity;

    bool GetOneAndDelete<T>(Expression<Func<T, bool>> where) where T : Entity;
    bool GetListAndDelete<T>(Expression<Func<T, bool>> where) where T : Entity;

    bool GetOneAndRestore<T>(Expression<Func<T, bool>> where) where T : Entity;
    bool GetListAndRestore<T>(Expression<Func<T, bool>> where) where T : Entity;

    void HardDelete<T>(T item) where T : Entity;
    void HardDeleteRange<T>(List<T> range) where T : Entity;

    bool GetOneAndHardDelete<T>(Expression<Func<T, bool>> where) where T : Entity;
    bool GetListAndHardDelete<T>(Expression<Func<T, bool>> where) where T : Entity;

    #endregion

    #region [UnitOfEWork]
    void Attach<T>(T entity) where T : Entity;
    void SetState<T>(T entity, string state) where T : Entity;
    Task<int> SaveChangesAsync();

    #endregion
}

public class DBService : IDBService
{
    private readonly TechStoreDB _db;
    public DBService(TechStoreDB db)
    {
        _db = db;
    }

    #region Queries [Select]
    public T Find<T>(Guid id) where T : Entity
    {
        var entity = _db.Set<T>().Find(id);
        if(entity is not null)
            _db.Entry(entity).State = EntityState.Detached;
        return entity;
    }
    public T GetOne<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return _db.Set<T>()
            .AsNoTracking()
            .Where(where)
            .FirstOrDefault();
    }
    public T GetOne<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return query.Where(where).FirstOrDefault();
    }
    public T GetOne<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return query.Where(where).FirstOrDefault();
    }

    public List<T> GetAll<T>() where T : Entity
    {
        return _db.Set<T>()
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
    }
    public List<T> GetList<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return _db.Set<T>()
            .AsNoTracking()
            .Where(where)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
    }
    public List<T> GetList<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return query
            .Where(where)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
    }
    public List<T> GetList<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return query
            .Where(where)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
    }

    public PageResult<T> GetPage<T>(IQueryable<T> query, int pageSize = 10, int pageNumber = 1) where T : Entity
    {
        var count = query.Count();
        return new PageResult<T>
        {
            PageItems = query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .OrderByDescending(e => e.CreatedAt)
                .ToList(),
            TotalItems = count,
            TotalPages = (int) Math.Ceiling((decimal) count / pageSize),
        };
    }

    public int Count<T>() where T : Entity
    {
        return _db.Set<T>().Count();
    }
    public int Count<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return _db.Set<T>().Count(where);
    }

    public IQueryable<T> GetQuery<T>() where T : Entity
    {
        return _db.Set<T>().AsNoTracking();
    }
    public IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return _db.Set<T>()
            .AsNoTracking()
            .Where(where);
    }
    #endregion

    #region Commands [Add-Update-Delete]
    public T Add<T>(T item) where T : Entity
    {
        return _db.Set<T>().Add(item).Entity;
    }
    public void AddRange<T>(List<T> range) where T : Entity
    {
        _db.Set<T>().AddRange(range);
    }
    public List<T> AddAndGetRange<T>(List<T> range) where T : Entity
    {
        return range.Select(obj => _db.Set<T>().Add(obj).Entity)
            .ToList();
    }

    public T Update<T>(T item) where T : Entity
    {
        item.ModifiedBy = "app_dev";
        item.ModifiedAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
        return item;
    }
    public void UpdateRange<T>(List<T> range) where T : Entity
    {
        range.ForEach(item =>
        {
            item.ModifiedBy = "app_dev";
            item.ModifiedAt = DateTime.UtcNow;
            _db.Entry(item).State = EntityState.Modified;
        });
    }
    public List<T> UpdateAndGetRange<T>(List<T> range) where T : Entity
    {
        range.ForEach(item =>
        {
            item.ModifiedBy = "app_dev";
            item.ModifiedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
        return range;
    }

    public void Activate<T>(T item) where T : Entity
    {
        item.IsActive = true;
        item.ActivatedBy = "app_dev";
        item.ActivatedAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void ActivateRange<T>(List<T> range) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsActive = true;
            item.ActivatedBy = "app_dev";
            item.ActivatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }
    public void Disable<T>(T item) where T : Entity
    {
        item.IsActive = false;
        item.DisabledBy = "app_dev";
        item.DisabledAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void DisableRange<T>(List<T> range) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsActive = false;
            item.DisabledBy = "app_dev";
            item.DisabledAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }

    public void Delete<T>(T item) where T : Entity
    {
        item.IsDeleted = true;
        item.DeletedBy = "app_dev";
        item.DeletedAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void DeleteRange<T>(List<T> range) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsDeleted = true;
            item.DeletedBy = "app_dev";
            item.DeletedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }
    public void Restore<T>(T item) where T : Entity
    {
        item.IsDeleted = false;
        item.RestoredBy = "app_dev";
        item.RestoredAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void RestoreRange<T>(List<T> range) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsDeleted = false;
            item.RestoredBy = "app_dev";
            item.RestoredAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }

    public bool GetOneAndDelete<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var item = _db.Set<T>().Where(where).FirstOrDefault();
        if (item != null)
        {
            Delete<T>(item);
            return true;
        }
        return false;
    }
    public bool GetListAndDelete<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var items = _db.Set<T>().Where(where).ToList();
        if (items.Count != 0)
        {
            DeleteRange<T>(items);
            return true;
        }
        return false;
    }
    public bool GetOneAndRestore<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var item = _db.Set<T>().Where(where).FirstOrDefault();
        if (item != null)
        {
            Restore<T>(item);
            return true;
        }
        return false;
    }
    public bool GetListAndRestore<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var items = _db.Set<T>().Where(where).ToList();
        if (items.Count != 0)
        {
            RestoreRange<T>(items);
            return true;
        }
        return false;
    }

    public void HardDelete<T>(T item) where T : Entity
    {
        _db.Set<T>().Remove(item);
    }
    public void HardDeleteRange<T>(List<T> range) where T : Entity
    {
        _db.Set<T>().RemoveRange(range);
    }
    public bool GetOneAndHardDelete<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var set = _db.Set<T>();
        T item = set.Where(where).FirstOrDefault();
        if (item != null)
        {
            set.Remove(item);
            return true;
        }
        return false;
    }
    public bool GetListAndHardDelete<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var set = _db.Set<T>();
        var items = set.Where(where).ToList();
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
