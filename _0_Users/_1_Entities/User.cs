using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public enum Gender : byte
{
    Male = 1,
    Female = 2
}

[Table("Users")]
public class User : IdentityUser
{
    [Required]
    [Column("FirstName", TypeName="nvarchar(30)")]
    public string FirstName { get; set; }

    [Required]
    [Column("LastName", TypeName="nvarchar(100)")]
    public string LastName { get; set; }

    [Required]
    [Column("BirthDate", TypeName="datetime2(3)")]
    public DateTime BirthDate { get; set; }

    [Required]
    [Column("Gender", TypeName="tinyint")]
    public Gender Gender { get; set; }

    public virtual ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
    public virtual ICollection<Sale> EmployeeSales { get; set; } = new HashSet<Sale>();
    public virtual ICollection<Sale> CustomerSales { get; set; } = new HashSet<Sale>();
}
