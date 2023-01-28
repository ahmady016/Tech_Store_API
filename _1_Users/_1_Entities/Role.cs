using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStoreApi.Entities;

[Table("Roles")]
public class Role : IdentityRole<Guid>
{
    [Key]
    [Column("Id", TypeName="uniqueidentifier")]
    public override Guid Id { get; set; }

    [Column("Description", TypeName="nvarchar(500)")]
    public string Description { get; set; }

    public virtual ICollection<UserRole> Users { get; set; } = new HashSet<UserRole>();
}
