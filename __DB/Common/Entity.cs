using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TechStoreApi.DB.Common;

public abstract class Entity
{
    public virtual Guid Id { get; set; }

    public virtual DateTime CreatedAt { get; set; }
    public virtual string CreatedBy { get; set; }

    public virtual DateTime? ModifiedAt { get; set; }
    public virtual string ModifiedBy { get; set; }

    public virtual bool IsDeleted { get; set; } = false;
    public virtual DateTime? DeletedAt { get; set; }
    public virtual string DeletedBy { get; set; }
    public virtual DateTime? RestoredAt { get; set; }
    public virtual string RestoredBy { get; set; }

    public virtual bool IsActive { get; set; } = false;
    public virtual DateTime? ActivatedAt { get; set; }
    public virtual string ActivatedBy { get; set; }
    public virtual DateTime? DisabledAt { get; set; }
    public virtual string DisabledBy { get; set; }
}

public abstract class EntityConfig<T> : IEntityTypeConfiguration<T> where T : Entity
{
    public virtual void Configure(EntityTypeBuilder<T> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()")
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIME()")
            .HasColumnName("created_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.CreatedBy)
            .IsRequired()
            .HasDefaultValue("app_dev")
            .HasMaxLength(100)
            .HasColumnName("created_by")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.ModifiedAt)
            .HasColumnName("modified_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.ModifiedBy)
            .HasMaxLength(100)
            .HasColumnName("modified_by")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_deleted")
            .HasColumnType("bit");

        entity.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.DeletedBy)
            .HasMaxLength(100)
            .HasColumnName("deleted_by")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.RestoredAt)
            .HasColumnName("restored_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.RestoredBy)
            .HasMaxLength(100)
            .HasColumnName("restored_by")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_active")
            .HasColumnType("bit");

        entity.Property(e => e.ActivatedAt)
            .HasColumnName("activated_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.ActivatedBy)
            .HasMaxLength(100)
            .HasColumnName("activated_by")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.DisabledAt)
            .HasColumnName("disabled_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.DisabledBy)
            .HasMaxLength(100)
            .HasColumnName("disabled_by")
            .HasColumnType("varchar(100)");
    }
}
