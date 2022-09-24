using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DB.Common;

namespace Entities;

public class Model : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbUrl { get; set; }
    public string PhotoUrl { get; set; }
    public Category Category { get; set; }
    public Guid ProductId { get; set; }
    public Guid BrandId { get; set; }

    public Product Product { get; set; }
    public Brand Brand { get; set; }
    public Stock Stock { get; set; }
    public virtual ICollection<PurchaseItem> PurchasesItems { get; set; } = new HashSet<PurchaseItem>();
    public virtual ICollection<SaleItem> SalesItems { get; set; } = new HashSet<SaleItem>();
}

public class ModelConfig : EntityConfig<Model>
{
    public override void Configure(EntityTypeBuilder<Model> entity)
    {
        entity.ToTable("models");
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("title")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(400)
            .HasColumnName("description")
            .HasColumnType("varchar(400)");

        entity.Property(e => e.ThumbUrl)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("thumb_url")
            .HasColumnType("varchar(500)");

        entity.Property(e => e.PhotoUrl)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("photo_url")
            .HasColumnType("varchar(500)");

        entity.Property(e => e.Category)
            .IsRequired()
            .HasColumnName("category")
            .HasColumnType("tinyint");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("model_title_unique_index")
            .IsUnique();

        entity.Property(e => e.ProductId)
            .HasColumnName("product_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.BrandId)
            .HasColumnName("brand_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.ProductId, "product_id_fk_index");
        entity.HasOne(model => model.Product)
            .WithMany(product => product.Models)
            .HasForeignKey(model => model.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("products_models_fk");

        entity.HasIndex(e => e.BrandId, "brand_id_fk_index");
        entity.HasOne(model => model.Brand)
            .WithMany(brand => brand.Models)
            .HasForeignKey(model => model.BrandId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("brands_models_fk");

        entity.HasOne(model => model.Stock)
            .WithOne(stock => stock.Model)
            .HasForeignKey<Stock>(stock => stock.ModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("models_stocks_fk");

    }
}
