using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ModelId { get; set; }
    public bool IsApproved { get; set; } = false;
    public string ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

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
            .HasMaxLength(100)
            .HasColumnName("approved_by")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.ApprovedAt)
            .HasColumnName("approved_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.CustomerId)
            .HasColumnName("customer_id")
            .HasColumnType("uniqueidentifier");

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

public class CommentFaker : Faker<Comment> {
    public CommentFaker()
    {
        RuleFor(o => o.Text, f => f.Lorem.Sentences(f.Random.Int(2, 8)));
        RuleFor(o => o.IsApproved, f => f.Random.Bool().OrDefault(f, 0.3f));
        RuleFor(o => o.ApprovedAt, f => f.Date.Between(f.Date.Past(2), DateTime.UtcNow).OrNull(f, 0.3f));
        RuleFor(o => o.ApprovedBy, f => f.Internet.Email().OrNull(f, 0.3f));
    }
}
