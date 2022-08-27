using Common;
using DB.Common;

namespace DB;

public interface ICrudService
{
    List<TDto> List<T, TDto>(string type = "existed") where T : Entity;
    PageResult<TDto> ListPage<T, TDto>(string type = "existed", int pageSize = 10, int pageNumber = 1) where T : Entity;

    IResult Query<T, TDto>(string where, string select, string orderBy) where T : Entity;
    IResult QueryPage<T, TDto>(string where, string select, string orderBy, int pageSize = 10, int pageNumber = 1) where T : Entity;

    TDto Find<T, TDto>(Guid id) where T : Entity;
    List<TDto> FindList<T, TDto>(string ids) where T : Entity;

    TDto Add<T, TDto, TCreateInput>(TCreateInput input) where T : Entity;
    List<TDto> AddMany<T, TDto, TCreateInput>(List<TCreateInput> inputs) where T : Entity;

    TDto Update<T, TDto, TUpdateInput>(TUpdateInput input) where T : Entity where TUpdateInput : IdInput;
    List<TDto> UpdateMany<T, TDto, TUpdateInput>(List<TUpdateInput> inputs) where T : Entity where TUpdateInput : IdInput;

    bool Delete<T>(Guid id) where T : Entity;
    bool Restore<T>(Guid id) where T : Entity;

    bool Activate<T>(Guid id) where T : Entity;
    bool Disable<T>(Guid id) where T : Entity;
}
