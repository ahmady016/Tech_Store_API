using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TechStoreApi.DB.Common;

namespace TechStoreApi.Entities;

[Table("Users")]
public class User : IdentityUser<Guid>
{
    [Key]
    [Column("Id", TypeName="uniqueidentifier")]
    public override Guid Id { get; set; }

    [Required]
    [Column("FullName", TypeName="nvarchar(50)")]
    public string FullName { get; set; }

    [Required]
    [Column("GenderId", TypeName="tinyint")]
    public Gender GenderId { get; set; }

    [Column("BirthDate", TypeName="datetime2(3)")]
    public DateTime? BirthDate { get; set; }

    [Required]
    [Column("StatusId", TypeName="tinyint")]
    [DefaultValue(AccountStatuses.Pending)]
    public AccountStatuses StatusId { get; set; } = AccountStatuses.Pending;

    [Column("LastLoggedAt", TypeName="datetime2(3)")]
    public DateTime? LastLoggedAt { get; set; }

    public virtual ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
    public virtual ICollection<Sale> EmployeeSales { get; set; } = new HashSet<Sale>();
    public virtual ICollection<Sale> CustomerSales { get; set; } = new HashSet<Sale>();

    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Reply> Replies { get; set; } = new HashSet<Reply>();

    public virtual ICollection<CustomerFavoriteModel> CustomersFavoritesModels { get; set; } = new HashSet<CustomerFavoriteModel>();
    public virtual ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
