﻿using System.Linq.Expressions;
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
    int SaveChanges();

    #endregion
}
