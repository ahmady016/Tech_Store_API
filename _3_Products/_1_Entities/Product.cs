using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

using TechStoreApi.DB.Common;

namespace TechStoreApi.Entities;

public class Product : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string PhotoName { get; set; }
    public string ThumbName { get; set; }
    public ProductCategories CategoryId { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new HashSet<Model>();
}

public class ProductConfig : EntityConfig<Product>
{
    public override void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.ToTable("products");
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

        entity.Property(e => e.PhotoName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("photo_url")
            .HasColumnType("varchar(50)");

        entity.Property(e => e.ThumbName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("thumb_url")
            .HasColumnType("varchar(50)");

        entity.Property(e => e.CategoryId)
            .IsRequired()
            .HasColumnName("category_id")
            .HasColumnType("tinyint");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("product_title_unique_index")
            .IsUnique();

    }
}

public class ProductFaker : Faker<Product> {
    private short counter = 1;
    public ProductFaker()
    {
        RuleFor(o => o.Title, f => $"{counter++}_{f.Commerce.ProductName()}");
        RuleFor(o => o.Description, f => f.Commerce.ProductDescription());
        RuleFor(o => o.PhotoName, f => f.Image.PicsumUrl());
        RuleFor(o => o.ThumbName, f => f.Image.PicsumUrl());
        RuleFor(o => o.CategoryId, f => f.PickRandom<ProductCategories>());
    }
}
