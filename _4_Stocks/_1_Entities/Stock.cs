using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

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

    public void PurchaseUpdate(int quantity, double totalPrice)
    {
        TotalPurchasesQuantity += quantity;
        TotalInStock += quantity;
        TotalPurchasesPrice += totalPrice;
        Profit -= totalPrice;
    }
    public void SaleUpdate(int quantity, double totalPrice)
    {
        TotalSalesQuantity += quantity;
        TotalInStock += quantity;
        TotalSalesPrice += totalPrice;
        Profit -= totalPrice;
    }
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

public class StockFaker : Faker<Stock> {
    public StockFaker()
    {
        RuleFor(o => o.TotalPurchasesPrice, f => f.Random.Double(10000, 100000));
        RuleFor(o => o.TotalSalesPrice, f => f.Random.Double(20000, 200000));
        RuleFor(o => o.Profit, f => f.Random.Double(10000, 100000));
        RuleFor(o => o.TotalPurchasesQuantity, f => f.Random.Long(1000, 10000));
        RuleFor(o => o.TotalSalesQuantity, f => f.Random.Long(800, 8000));
        RuleFor(o => o.TotalInStock, f => f.Random.Long(200, 2000));
    }
}
