using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public DateTime SoldAt { get; set; }
    public double TotalPrice { get; set; } = 0.0;
    public Guid EmployeeId { get; set; }
    public Guid CustomerId { get; set; }

    public User Employee { get; set; }
    public User Customer { get; set; }
    public virtual ICollection<SaleItem> Items { get; set; } = new HashSet<SaleItem>();
}

public class SaleConfig : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> entity)
    {
        entity.ToTable("sales");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()")
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.SoldAt)
            .IsRequired()
            .HasColumnName("sold_at")
            .HasColumnType("datetime2(3)");

        entity.Property(e => e.TotalPrice)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnName("total_price")
            .HasColumnType("float");

        entity.Property(e => e.EmployeeId)
            .IsRequired()
            .HasMaxLength(450)
            .HasColumnName("employee_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.CustomerId)
            .IsRequired()
            .HasMaxLength(450)
            .HasColumnName("customer_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.EmployeeId, "employee_id_fk_index");
        entity.HasOne(sale => sale.Employee)
            .WithMany(user => user.EmployeeSales)
            .HasForeignKey(sale => sale.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("users_employee_sales_fk");

        entity.HasIndex(e => e.CustomerId, "customer_id_fk_index");
        entity.HasOne(sale => sale.Customer)
            .WithMany(user => user.CustomerSales)
            .HasForeignKey(sale => sale.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("users_customer_sales_fk");

    }
}

public class SaleFaker : Faker<Sale> {
    public SaleFaker()
    {
        RuleFor(o => o.SoldAt, f => f.Date.Between(f.Date.Past(2), DateTime.UtcNow));
        RuleFor(o => o.TotalPrice, f => f.Random.Double(100, 10000));
    }
}
