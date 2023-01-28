using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStoreApi.Entities;

[Table("UsersLogins")]
public class UserLogin : IdentityUserLogin<Guid>
{
    [Key]
    [Column("Id", TypeName="uniqueidentifier")]
    public Guid Id { get; set; }
}
