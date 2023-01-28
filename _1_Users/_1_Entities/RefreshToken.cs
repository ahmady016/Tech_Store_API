using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStoreApi.Entities;

[Index(nameof(Value), IsUnique = true)]
[Table("RefreshTokens")]
public class RefreshToken
{
    [Key]
    [Column("Id", TypeName="uniqueidentifier")]
    public Guid Id { get; set; }

    [Required]
    [Column("Value", TypeName="varchar(128)")]
    public string Value { get; set; }

    [Required]
    [Column("ExpiresAt", TypeName="datetime2(3)")]
    public DateTime ExpiresAt { get; set; }

    [Column("RevokedAt", TypeName="datetime2(3)")]
    public DateTime? RevokedAt { get; set; }

    [Column("RevokedReason", TypeName="varchar(450)")]
    public string RevokedReason { get; set; }

    [Required]
    [Column("UserId", TypeName="uniqueidentifier")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [NotMapped]
    public bool IsRevoked => RevokedAt != null;

    [NotMapped]
    public bool IsExpired => DateTime.Now >= ExpiresAt;

    [NotMapped]
    public bool IsValid => !IsRevoked && !IsExpired;
}
