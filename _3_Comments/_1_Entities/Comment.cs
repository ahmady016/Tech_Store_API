using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsApproved { get; set; } = false;
    public string ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid ModelId { get; set; }
    public string CustomerId { get; set; }

    public Model Model { get; set; }
    public User Customer { get; set; }
    public virtual ICollection<Reply> Replies { get; set; } = new HashSet<Reply>();
}

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> entity)
    {
        entity.ToTable("comments");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Text)
            .IsRequired()
            .HasMaxLength(2000)
            .HasColumnName("text")
            .HasColumnType("nvarchar(2000)");

        entity.Property(e => e.IsApproved)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_approved")
            .HasColumnType("bit");

        entity.Property(e => e.ApprovedBy)
            .HasMaxLength(450)
            .HasColumnName("approved_by")
            .HasColumnType("nvarchar(450)");

        entity.Property(e => e.ApprovedAt)
            .HasColumnName("approved_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.CustomerId)
            .HasColumnName("customer_id")
            .HasColumnType("nvarchar(450)");

        entity.HasIndex(e => e.ModelId, "comments_model_id_fk_index");
        entity.HasOne(comment => comment.Model)
            .WithMany(model => model.Comments)
            .HasForeignKey(comment => comment.ModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("models_comments_fk");

        entity.HasIndex(e => e.CustomerId, "comments_customer_id_fk_index");
        entity.HasOne(comment => comment.Customer)
            .WithMany(user => user.Comments)
            .HasForeignKey(comment => comment.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("users_comments_fk");

    }
}
