using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

public class Purchase
{
    public Guid Id { get; set; }
    public DateTime PurchasedAt { get; set; }
    public double TotalPrice { get; set; } = 0.0;
    public Guid EmployeeId { get; set; }

    public User Employee { get; set; }
    public virtual ICollection<PurchaseItem> Items { get; set; } = new HashSet<PurchaseItem>();
}

public class PurchaseConfig : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> entity)
    {
        entity.ToTable("purchases");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()")
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.PurchasedAt)
            .IsRequired()
            .HasColumnName("purchased_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.TotalPrice)
            .IsRequired()
            .HasColumnName("total_price")
            .HasColumnType("float");

        entity.Property(e => e.EmployeeId)
            .IsRequired()
            .HasMaxLength(450)
            .HasColumnName("employee_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.EmployeeId, "employee_id_fk_index");
        entity.HasOne(purchase => purchase.Employee)
            .WithMany(user => user.Purchases)
            .HasForeignKey(purchase => purchase.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("users_purchases_fk");

    }
}

public class PurchaseFaker : Faker<Purchase> {
    public PurchaseFaker()
    {
        RuleFor(o => o.PurchasedAt, f => f.Date.Between(f.Date.Past(2), DateTime.UtcNow));
        RuleFor(o => o.TotalPrice, f => f.Random.Double(100, 10000));
    }
}
