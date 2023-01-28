
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TechStoreApi.Entities;

[Table("UsersTokens")]
public class UserToken : IdentityUserToken<Guid>
{
    [Key]
    [Column("Id", TypeName="uniqueidentifier")]
    public Guid Id { get; set; }
}
