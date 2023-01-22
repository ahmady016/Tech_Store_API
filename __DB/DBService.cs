using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

using TechStoreApi.Common;
using TechStoreApi.DB.Common;

namespace TechStoreApi.DB;

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
    Task<PageResult<dynamic>> GetPageAsync(IQueryable query, int pageSize = 10, int pageNumber = 1);

    Task<int> CountAsync<T>() where T : Entity;
    Task<int> CountAsync<T>(Expression<Func<T, bool>> where) where T : Entity;

    IQueryable<T> GetQuery<T>() where T : Entity;
    IQueryable<T> GetQueryWithTracking<T>() where T : Entity;

    #endregion

    #region Commands [Add-Update-Delete]
    void Add<T>(T item, string createdBy) where T : Entity;
    void AddRange<T>(List<T> range, string createdBy) where T : Entity;

    void Update<T>(T item, string modifiedBy) where T : Entity;
    void UpdateRange<T>(List<T> range, string modifiedBy) where T : Entity;

    void Remove<T>(T item) where T : Entity;
    void RemoveRange<T>(List<T> range) where T : Entity;

    void Activate<T>(T item, string activatedBy) where T : Entity;
    void ActivateRange<T>(List<T> range, string activatedBy) where T : Entity;
    void Disable<T>(T item, string disabledBy) where T : Entity;
    void DisableRange<T>(List<T> range, string disabledBy) where T : Entity;

    void Delete<T>(T item, string deletedBy) where T : Entity;
    void DeleteRange<T>(List<T> range, string deletedBy) where T : Entity;
    void Restore<T>(T item, string restoredBy) where T : Entity;
    void RestoreRange<T>(List<T> range, string restoredBy) where T : Entity;

    Task<bool> GetOneAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : Entity;
    Task<bool> GetListAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : Entity;

    Task<bool> GetOneAndActivateAsync<T>(Expression<Func<T, bool>> where, string activatedBy) where T : Entity;
    Task<bool> GetListAndActivateAsync<T>(Expression<Func<T, bool>> where, string activatedBy) where T : Entity;
    Task<bool> GetOneAndDisableAsync<T>(Expression<Func<T, bool>> where, string disabledBy) where T : Entity;
    Task<bool> GetListAndDisableAsync<T>(Expression<Func<T, bool>> where, string disabledBy) where T : Entity;

    Task<bool> GetOneAndDeleteAsync<T>(Expression<Func<T, bool>> where, string deletedBy) where T : Entity;
    Task<bool> GetListAndDeleteAsync<T>(Expression<Func<T, bool>> where, string deletedBy) where T : Entity;
    Task<bool> GetOneAndRestoreAsync<T>(Expression<Func<T, bool>> where, string restoredBy) where T : Entity;
    Task<bool> GetListAndRestoreAsync<T>(Expression<Func<T, bool>> where, string restoredBy) where T : Entity;

    Task<int> ExecuteUpdateAsync<T>(Expression<Func<T, bool>> where, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setter) where T : Entity;
    Task<int> ExecuteDeleteAsync<T>(Expression<Func<T, bool>> where) where T : Entity;

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
    public DBService(TechStoreDB db) => _db = db;

    #region Queries [Select]
    public async Task<T> FindAsync<T>(Guid id) where T : Entity
    {
        return await _db.Set<T>().FindAsync(id);
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
    public IQueryable<T> GetQueryWithTracking<T>() where T : Entity
    {
        return _db.Set<T>();
    }

    #endregion

    #region Commands [Add-Update-Delete]
    public void Add<T>(T item, string createdBy = AppConstants.defaultUser) where T : Entity
    {
        item.CreatedAt= DateTime.UtcNow;
        item.CreatedBy = createdBy;
        _db.Set<T>().Add(item);
    }
    public void AddRange<T>(List<T> range, string createdBy = AppConstants.defaultUser) where T : Entity
    {
        range.ForEach(item =>
        {
            item.CreatedAt= DateTime.Now;
            item.CreatedBy = createdBy;
        });
        _db.Set<T>().AddRange(range);
    }

    public void Update<T>(T item, string modifiedBy = AppConstants.defaultUser) where T : Entity
    {
        item.ModifiedAt = DateTime.UtcNow;
        item.ModifiedBy = modifiedBy;
        _db.Set<T>().Update(item);
    }
    public void UpdateRange<T>(List<T> range, string modifiedBy = AppConstants.defaultUser) where T : Entity
    {
        range.ForEach(item =>
        {
            item.ModifiedAt = DateTime.UtcNow;
            item.ModifiedBy = modifiedBy;
        });
        _db.Set<T>().UpdateRange(range);
    }

    public void Activate<T>(T item, string activatedBy = AppConstants.defaultUser) where T : Entity
    {
        item.IsActive = true;
        item.ActivatedAt = DateTime.UtcNow;
        item.ActivatedBy = activatedBy;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void ActivateRange<T>(List<T> range, string activatedBy = AppConstants.defaultUser) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsActive = true;
            item.ActivatedAt = DateTime.UtcNow;
            item.ActivatedBy = activatedBy;
            _db.Entry(item).State = EntityState.Modified;
        });
    }
    public void Disable<T>(T item, string disabledBy = AppConstants.defaultUser) where T : Entity
    {
        item.IsActive = false;
        item.DisabledAt = DateTime.UtcNow;
        item.DisabledBy = disabledBy;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void DisableRange<T>(List<T> range, string disabledBy = AppConstants.defaultUser) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsActive = false;
            item.DisabledAt = DateTime.UtcNow;
            item.DisabledBy = disabledBy;
            _db.Entry(item).State = EntityState.Modified;
        });
    }

    public void Delete<T>(T item, string deletedBy = AppConstants.defaultUser) where T : Entity
    {
        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.DeletedBy = deletedBy;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void DeleteRange<T>(List<T> range, string deletedBy = AppConstants.defaultUser) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = deletedBy;
            _db.Entry(item).State = EntityState.Modified;
        });
    }
    public void Restore<T>(T item, string restoredBy = AppConstants.defaultUser) where T : Entity
    {
        item.IsDeleted = false;
        item.RestoredAt = DateTime.UtcNow;
        item.RestoredBy = restoredBy;
        _db.Entry(item).State = EntityState.Modified;
    }
    public void RestoreRange<T>(List<T> range, string restoredBy = AppConstants.defaultUser) where T : Entity
    {
        range.ForEach(item =>
        {
            item.IsDeleted = false;
            item.RestoredAt = DateTime.UtcNow;
            item.RestoredBy = restoredBy;
            _db.Entry(item).State = EntityState.Modified;
        });
    }

    public void Remove<T>(T item) where T : Entity
    {
        _db.Set<T>().Remove(item);
    }
    public void RemoveRange<T>(List<T> range) where T : Entity
    {
        _db.Set<T>().RemoveRange(range);
    }

    public async Task<bool> GetOneAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : Entity
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
    public async Task<bool> GetListAndRemoveAsync<T>(Expression<Func<T, bool>> where) where T : Entity
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

    public async Task<bool> GetOneAndActivateAsync<T>(Expression<Func<T, bool>> where, string activatedBy = AppConstants.defaultUser) where T : Entity
    {
        var item = await _db.Set<T>().Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            Activate<T>(item, activatedBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndActivateAsync<T>(Expression<Func<T, bool>> where, string activatedBy = AppConstants.defaultUser) where T : Entity
    {
        var set = _db.Set<T>();
        var items = await set.Where(where).ToListAsync();
        if (items.Count != 0)
        {
            ActivateRange(items, activatedBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetOneAndDisableAsync<T>(Expression<Func<T, bool>> where, string disabledBy = AppConstants.defaultUser) where T : Entity
    {
        var item = await _db.Set<T>().Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            Disable<T>(item, disabledBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndDisableAsync<T>(Expression<Func<T, bool>> where, string disabledBy = AppConstants.defaultUser) where T : Entity
    {
        var set = _db.Set<T>();
        var items = await set.Where(where).ToListAsync();
        if (items.Count != 0)
        {
            DisableRange(items, disabledBy);
            return true;
        }
        return false;
    }

    public async Task<bool> GetOneAndDeleteAsync<T>(Expression<Func<T, bool>> where, string deletedBy = AppConstants.defaultUser) where T : Entity
    {
        var item = await _db.Set<T>().Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            Delete<T>(item, deletedBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndDeleteAsync<T>(Expression<Func<T, bool>> where, string deletedBy = AppConstants.defaultUser) where T : Entity
    {
        var items = await _db.Set<T>().Where(where).ToListAsync();
        if (items.Count != 0)
        {
            DeleteRange<T>(items, deletedBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetOneAndRestoreAsync<T>(Expression<Func<T, bool>> where, string restoredBy = AppConstants.defaultUser) where T : Entity
    {
        var item = await _db.Set<T>().Where(where).FirstOrDefaultAsync();
        if (item != null)
        {
            Restore<T>(item, restoredBy);
            return true;
        }
        return false;
    }
    public async Task<bool> GetListAndRestoreAsync<T>(Expression<Func<T, bool>> where, string restoredBy = AppConstants.defaultUser) where T : Entity
    {
        var items = await _db.Set<T>().Where(where).ToListAsync();
        if (items.Count != 0)
        {
            RestoreRange<T>(items, restoredBy);
            return true;
        }
        return false;
    }

    public async Task<int> ExecuteUpdateAsync<T>(Expression<Func<T, bool>> where, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setter) where T : Entity
    {
        return await _db.Set<T>().Where(where).ExecuteUpdateAsync(setter);
    }
    public async Task<int> ExecuteDeleteAsync<T>(Expression<Func<T, bool>> where) where T : Entity
    {
        return await _db.Set<T>().Where(where).ExecuteDeleteAsync();
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
