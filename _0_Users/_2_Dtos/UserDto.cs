using Entities;

namespace Dtos;
public class UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; }
    public string PhoneNumberConfirmed { get; set; }
}
