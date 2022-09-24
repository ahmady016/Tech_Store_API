using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class PurchaseItem
{
    public Guid Id { get; set; }
    public int Quantity { get; set; } = 0;
    public double UnitPrice { get; set; } = 0.0;
    public double TotalPrice { get; set; } = 0.0;
    public Guid ModelId { get; set; }
    public Guid PurchaseId { get; set; }

    public Model Model { get; set; }
    public Purchase Purchase { get; set; }
}

public class PurchaseItemConfig : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> entity)
    {
        entity.ToTable("purchases_items");
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

        entity.Property(e => e.PurchaseId)
            .IsRequired()
            .HasColumnName("purchase_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.ModelId, "model_id_fk_index");
        entity.HasOne(purchaseItem => purchaseItem.Model)
            .WithMany(model => model.PurchasesItems)
            .HasForeignKey(purchaseItem => purchaseItem.ModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("models_purchases_items_fk");

        entity.HasIndex(e => e.PurchaseId, "purchase_id_fk_index");
        entity.HasOne(purchaseItem => purchaseItem.Purchase)
            .WithMany(purchase => purchase.Items)
            .HasForeignKey(purchaseItem => purchaseItem.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("purchases_purchases_items_fk");

    }
}
