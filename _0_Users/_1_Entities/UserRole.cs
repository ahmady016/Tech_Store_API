using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

[Table("UsersRoles")]
public class UserRole
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string RoleId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [ForeignKey("RoleId")]
    public IdentityRole Role { get; set; }
}

public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(p => new { p.UserId, p.RoleId });
    }
}
