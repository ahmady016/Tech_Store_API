using TechStoreApi.DB.Common;

namespace TechStoreApi.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<RoleDto> Roles { get; set; }
}
