using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

[Table("RefreshTokens")]
[Index(nameof(Value), IsUnique = true)]
public class RefreshToken
{
    [Key]
    [Column("Id", TypeName="uniqueidentifier")]
    public Guid Id { get; set; }

    [Required]
    [Column("Value", TypeName="varchar(450)")]
    public string Value { get; set; }

    [Required]
    [Column("ExpiresAt", TypeName="datetime2(3)")]
    public DateTime ExpiresAt { get; set; }

    [Column("RevokedAt", TypeName="datetime2(3)")]
    public DateTime? RevokedAt { get; set; }

    [Column("RevokedReason", TypeName="varchar(450)")]
    public string RevokedReason { get; set; }

    [Required]
    [Column("UserId", TypeName="nvarchar(450)")]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [NotMapped]
    public bool IsRevoked => RevokedAt != null;
    [NotMapped]
    public bool IsExpired => DateTime.Now >= ExpiresAt;
    [NotMapped]
    public bool IsValid => !IsRevoked && !IsExpired;

    [NotMapped]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [NotMapped]
    public string CreatedBy { get; set; } = "app_dev";
    [NotMapped]
    public DateTime? ModifiedAt { get; set; }
    [NotMapped]
    public string ModifiedBy { get; set; }

    [NotMapped]
    public bool IsDeleted { get; set; } = false;
    [NotMapped]
    public DateTime? DeletedAt { get; set; }
    [NotMapped]
    public string DeletedBy { get; set; }
    [NotMapped]
    public DateTime? RestoredAt { get; set; }
    [NotMapped]
    public string RestoredBy { get; set; }

    [NotMapped]
    public bool IsActive { get; set; } = false;
    [NotMapped]
    public DateTime? ActivatedAt { get; set; }
    [NotMapped]
    public string ActivatedBy { get; set; }
    [NotMapped]
    public DateTime? DisabledAt { get; set; }
    [NotMapped]
    public string DisabledBy { get; set; }
}
