namespace Dtos;
public class RoleDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<UserDto> Users { get; set; }
}
