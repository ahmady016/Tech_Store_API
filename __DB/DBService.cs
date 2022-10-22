using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using DB.Common;

namespace DB;

public interface IDBService
{
    #region Query [Select]
    Task<T> FindAsync<T>(Guid id) where T : Entity;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where) where T : Entity;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity;
    Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity;

    Task<List<T>> GetAllAsync<T>() where T : Entity;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : Entity;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity;
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity;
    Task<PageResult<T>> GetPageAsync<T>(IQueryable<T> query, int pageSize, int pageNumber) where T : Entity;

    Task<int> CountAsync<T>() where T : Entity;
    Task<int> CountAsync<T>(Expression<Func<T, bool>> where) where T : Entity;

    IQueryable<T> GetQuery<T>() where T : Entity;
    IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> where) where T : Entity;

    #endregion

    #region Commands [Add-Update-Delete]
    void Add<T>(T item, string createdBy = "app_dev") where T : Entity;
    void AddRange<T>(List<T> range, string createdBy = "app_dev") where T : Entity;
    T AddAndGetOne<T>(T item, string createdBy = "app_dev") where T : Entity;
    List<T> AddAndGetRange<T>(List<T> range, string createdBy = "app_dev") where T : Entity;

    void Update<T>(T item, string modifiedBy = "app_dev") where T : Entity;
    void UpdateRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity;

    void Activate<T>(T item, string modifiedBy = "app_dev") where T : Entity;
    void ActivateRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity;

    void Disable<T>(T item, string modifiedBy = "app_dev") where T : Entity;
    void DisableRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity;

    void Delete<T>(T item, string modifiedBy = "app_dev") where T : Entity;
    void DeleteRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity;

    void Restore<T>(T item, string modifiedBy = "app_dev") where T : Entity;
    void RestoreRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity;

    Task<bool> GetOneAndDeleteAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity;
    Task<bool> GetListAndDeleteAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity;

    Task<bool> GetOneAndRestoreAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity;
    Task<bool> GetListAndRestoreAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity;

    void HardDelete<T>(T item) where T : Entity;
    void HardDeleteRange<T>(List<T> range) where T : Entity;

    Task<bool> GetOneAndHardDeleteAsync<T>(Expression<Func<T, bool>> where) where T : Entity;
    Task<bool> GetListAndHardDeleteAsync<T>(Expression<Func<T, bool>> where) where T : Entity;

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
    public async Task<T> FindAsync<T>(Guid id) where T : Entity
    {
        var entity = await _db.Set<T>().FindAsync(id);
        if(entity is not null)
            _db.Entry(entity).State = EntityState.Detached;
        return entity;
    }
    public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return await _db.Set<T>()
            .AsNoTracking()
            .Where(where)
            .FirstOrDefaultAsync();
    }
    public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.Where(where).FirstOrDefaultAsync();
    }
    public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.Where(where).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync<T>() where T : Entity
    {
        return await _db.Set<T>()
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return await _db.Set<T>()
            .AsNoTracking()
            .Where(where)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query
            .Where(where)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[] includes) where T : Entity
    {
        var query = _db.Set<T>().AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query
            .Where(where)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<PageResult<T>> GetPageAsync<T>(IQueryable<T> query, int pageSize = 10, int pageNumber = 1) where T : Entity
    {
        var count = await query.CountAsync();
        return new PageResult<T>
        {
            PageItems = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(),
            TotalItems = count,
            TotalPages = (int) Math.Ceiling((decimal) count / pageSize),
        };
    }

    public async Task<int> CountAsync<T>() where T : Entity
    {
        return await _db.Set<T>().CountAsync();
    }
    public async Task<int> CountAsync<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return await _db.Set<T>().CountAsync(where);
    }

    public IQueryable<T> GetQuery<T>() where T : Entity
    {
        return _db.Set<T>().AsNoTracking();
    }
    public IQueryable<T> GetQuery<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return _db.Set<T>().AsNoTracking().Where(where);
    }
    #endregion

    #region Commands [Add-Update-Delete]
    public void Add<T>(T item, string createdBy = "app_dev") where T : Entity
    {
        item.CreatedAt= DateTime.Now;
        item.CreatedBy = createdBy;
        _db.Set<T>().Add(item);
    }
    public void AddRange<T>(List<T> range, string createdBy = "app_dev") where T : Entity
    {
        range.ForEach(item => {
            item.CreatedAt= DateTime.Now;
            item.CreatedBy = createdBy;
        });
        _db.Set<T>().AddRange(range);
    }
    public T AddAndGetOne<T>(T item, string createdBy = "app_dev") where T : Entity
    {
        item.CreatedAt= DateTime.Now;
        item.CreatedBy = createdBy;
        return _db.Set<T>().Add(item).Entity;
    }
    public List<T> AddAndGetRange<T>(List<T> range, string createdBy = "app_dev") where T : Entity
    {
        range.ForEach(item => {
            item.CreatedAt= DateTime.Now;
            item.CreatedBy = createdBy;
        });
        return range.Select(obj => _db.Set<T>().Add(obj).Entity).ToList();
    }

    public void Update<T>(T item, string modifiedBy = "app_dev") where T : Entity
    {
        item.ModifiedBy = modifiedBy;
        item.ModifiedAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void UpdateRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity
    {
        range.ForEach(item =>
        {
            item.ModifiedBy = modifiedBy;
            item.ModifiedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }

    public void Activate<T>(T item, string modifiedBy = "app_dev") where T : Entity
    {
        item.IsActive = true;
        item.ActivatedBy = modifiedBy;
        item.ActivatedAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void ActivateRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsActive = true;
            item.ActivatedBy = modifiedBy;
            item.ActivatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }
    public void Disable<T>(T item, string modifiedBy = "app_dev") where T : Entity
    {
        item.IsActive = false;
        item.DisabledBy = modifiedBy;
        item.DisabledAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void DisableRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsActive = false;
            item.DisabledBy = modifiedBy;
            item.DisabledAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }

    public void Delete<T>(T item, string modifiedBy = "app_dev") where T : Entity
    {
        item.IsDeleted = true;
        item.DeletedBy = modifiedBy;
        item.DeletedAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void DeleteRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsDeleted = true;
            item.DeletedBy = modifiedBy;
            item.DeletedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }
    public void Restore<T>(T item, string modifiedBy = "app_dev") where T : Entity
    {
        item.IsDeleted = false;
        item.RestoredBy = modifiedBy;
        item.RestoredAt = DateTime.Now;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void RestoreRange<T>(List<T> range, string modifiedBy = "app_dev") where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsDeleted = false;
            item.RestoredBy = modifiedBy;
            item.RestoredAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
        });
    }

    public async Task<bool> GetOneAndDeleteAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity
    {
        var item = await _db.Set<T>().Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            Delete<T>(item, modifiedBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndDeleteAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity
    {
        var items = await _db.Set<T>().Where(where).ToListAsync();
        if (items.Count > 0)
        {
            DeleteRange<T>(items, modifiedBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetOneAndRestoreAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity
    {
        var item = await _db.Set<T>().Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            Restore<T>(item, modifiedBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndRestoreAsync<T>(Expression<Func<T, bool>> where, string modifiedBy = "app_dev") where T : Entity
    {
        var items = await _db.Set<T>().Where(where).ToListAsync();
        if (items.Count > 0)
        {
            RestoreRange<T>(items, modifiedBy);
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
    public async Task<bool> GetOneAndHardDeleteAsync<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var item = await _db.Set<T>().Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            HardDelete<T>(item);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndHardDeleteAsync<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        var items = await _db.Set<T>().Where(where).ToListAsync();
        if (items.Count > 0)
        {
            HardDeleteRange<T>(items);
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
