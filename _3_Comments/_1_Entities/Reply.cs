using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class Reply
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsApproved { get; set; } = false;
    public string ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid CommentId { get; set; }
    public string CustomerId { get; set; }

    public Comment Comment { get; set; }
    public User Customer { get; set; }
}

public class ReplyConfig : IEntityTypeConfiguration<Reply>
{
    public void Configure(EntityTypeBuilder<Reply> entity)
    {
        entity.ToTable("replies");
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

        entity.Property(e => e.CommentId)
            .HasColumnName("comment_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.CustomerId)
            .HasColumnName("customer_id")
            .HasColumnType("nvarchar(450)");

        entity.HasIndex(e => e.CommentId, "replies_comment_id_fk_index");
        entity.HasOne(Reply => Reply.Comment)
            .WithMany(comment => comment.Replies)
            .HasForeignKey(Reply => Reply.CommentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("comments_replies_fk");

        entity.HasIndex(e => e.CustomerId, "replies_customer_id_fk_index");
        entity.HasOne(Reply => Reply.Customer)
            .WithMany(user => user.Replies)
            .HasForeignKey(Reply => Reply.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("users_replies_fk");

    }
}
