using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class CustomerFavoriteModel
{
    public string CustomerId { get; set; }
    public Guid ModelId { get; set; }

    public Model Model { get; set; }
    public User Customer { get; set; }
}

public class CustomerFavoriteModelConfig : IEntityTypeConfiguration<CustomerFavoriteModel>
{
    public void Configure(EntityTypeBuilder<CustomerFavoriteModel> entity)
    {
        entity.ToTable("customers_favorites_models");
        entity.HasKey(e => new { e.CustomerId, e.ModelId });

        entity.Property(e => e.CustomerId)
            .HasColumnName("customer_id")
            .HasColumnType("nvarchar(450)");

        entity.Property(e => e.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.CustomerId, "customers_favorites_models_customer_id_fk_index");
        entity.HasOne(customerFavoriteModel => customerFavoriteModel.Customer)
            .WithMany(user => user.CustomersFavoritesModels)
            .HasForeignKey(customerFavoriteModel => customerFavoriteModel.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("users_customers_favorites_models_fk");

        entity.HasIndex(e => e.ModelId, "customers_favorites_models_model_id_fk_index");
        entity.HasOne(customerFavoriteModel => customerFavoriteModel.Model)
            .WithMany(model => model.CustomersFavoritesModels)
            .HasForeignKey(customerFavoriteModel => customerFavoriteModel.ModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("models_customers_favorites_models_fk");

    }
}
