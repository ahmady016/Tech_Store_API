using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class Rating
{
    public string CustomerId { get; set; }
    public Guid ModelId { get; set; }
    public byte Value { get; set; }

    public Model Model { get; set; }
    public User Customer { get; set; }
}

public class RatingConfig : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> entity)
    {
        entity.ToTable("ratings");
        entity.HasKey(e => new { e.CustomerId, e.ModelId });

        entity.Property(e => e.CustomerId)
            .IsRequired()
            .HasColumnName("customer_id")
            .HasColumnType("nvarchar(450)");

        entity.Property(e => e.ModelId)
            .IsRequired()
            .HasColumnName("model_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.Value)
            .IsRequired()
            .HasColumnName("value")
            .HasColumnType("tinyint");

        entity.HasIndex(e => e.CustomerId, "ratings_customer_id_fk_index");
        entity.HasOne(customerFavoriteModel => customerFavoriteModel.Customer)
            .WithMany(user => user.Ratings)
            .HasForeignKey(customerFavoriteModel => customerFavoriteModel.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("users_ratings_fk");

        entity.HasIndex(e => e.ModelId, "ratings_model_id_fk_index");
        entity.HasOne(customerFavoriteModel => customerFavoriteModel.Model)
            .WithMany(model => model.Ratings)
            .HasForeignKey(customerFavoriteModel => customerFavoriteModel.ModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("models_ratings_fk");

    }
}
