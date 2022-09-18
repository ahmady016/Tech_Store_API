using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Entities;

[Table("Roles")]
public class Role : IdentityRole
{
    public virtual ICollection<UserRole> Users { get; set; } = new HashSet<UserRole>();
}
