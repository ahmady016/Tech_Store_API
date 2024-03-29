using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

public class SaleItem
{
    public Guid Id { get; set; }
    public int Quantity { get; set; } = 0;
    public double UnitPrice { get; set; } = 0.0;
    public double TotalPrice { get; set; } = 0.0;
    public Guid ModelId { get; set; }
    public Guid SaleId { get; set; }

    public Model Model { get; set; }
    public Sale Sale { get; set; }
}

public class SaleItemConfig : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> entity)
    {
        entity.ToTable("sales_items");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()")
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.Quantity)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("quantity")
            .HasColumnType("int");

        entity.Property(e => e.UnitPrice)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnName("unit_price")
            .HasColumnType("float");

        entity.Property(e => e.TotalPrice)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnName("total_price")
            .HasColumnType("float");

        entity.Property(e => e.ModelId)
            .IsRequired()
            .HasColumnName("model_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.SaleId)
            .IsRequired()
            .HasColumnName("sale_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.ModelId, "sales_items_model_id_fk_index");
        entity.HasOne(saleItem => saleItem.Model)
            .WithMany(model => model.SalesItems)
            .HasForeignKey(saleItem => saleItem.ModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("models_sales_items_fk");

        entity.HasIndex(e => e.SaleId, "sales_items_sale_id_fk_index");
        entity.HasOne(saleItem => saleItem.Sale)
            .WithMany(sale => sale.Items)
            .HasForeignKey(saleItem => saleItem.SaleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("sales_sales_items_fk");

    }
}

public class SaleItemFaker : Faker<SaleItem> {
    public SaleItemFaker()
    {
        RuleFor(o => o.Quantity, f => f.Random.Int(1, 20));
        RuleFor(o => o.UnitPrice, f => f.Random.Double(5, 500));
        RuleFor(o => o.TotalPrice, f => f.Random.Double(100, 5000));
    }
}
