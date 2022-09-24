using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class Stock
{
    public Guid ModelId { get; set; }
    public double TotalPurchasesPrice { get; set; } = 0.0;
    public double TotalSalesPrice { get; set; } = 0.0;
    public double Profit { get; set; } = 0.0;
    public long TotalPurchasesQuantity { get; set; } = 0;
    public long TotalSalesQuantity { get; set; } = 0;
    public long TotalInStock { get; set; } = 0;

    public Model Model { get; set; }
}

public class StockConfig : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> entity)
    {
        entity.ToTable("stocks");
        entity.HasKey(e => e.ModelId);

        entity.Property(e => e.ModelId)
            .IsRequired()
            .HasColumnName("model_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.TotalPurchasesPrice)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnName("total_purchases_price")
            .HasColumnType("float");

        entity.Property(e => e.TotalSalesPrice)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnName("total_sales_price")
            .HasColumnType("float");

        entity.Property(e => e.Profit)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnName("profit")
            .HasColumnType("float");

        entity.Property(e => e.TotalPurchasesQuantity)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("total_purchases_quantity")
            .HasColumnType("bigint");

        entity.Property(e => e.TotalSalesQuantity)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("total_sales_quantity")
            .HasColumnType("bigint");

        entity.Property(e => e.TotalInStock)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("total_in_stock")
            .HasColumnType("bigint");

    }
}
