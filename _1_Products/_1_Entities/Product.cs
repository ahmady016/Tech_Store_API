using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DB.Common;

namespace Entities;

public enum Category : byte
{
    PCs = 1,
    Laptops = 2,
    Tablets = 3,
    Mobiles = 4,
    Accessories = 5
}

public class Product : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }

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

        entity.Property(e => e.Category)
            .IsRequired()
            .HasColumnName("category")
            .HasColumnType("tinyint");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("product_title_unique_index")
            .IsUnique();

    }
}
