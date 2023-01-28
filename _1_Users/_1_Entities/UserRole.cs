using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStoreApi.Entities;

[Table("UsersRoles")]
public class UserRole : IdentityUserRole<Guid>
{
    [Column("UserId", TypeName="uniqueidentifier")]
    public override Guid UserId { get; set; }

    [Column("RoleId", TypeName="uniqueidentifier")]
    public override Guid RoleId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}

public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(p => new { p.UserId, p.RoleId });
    }
}
