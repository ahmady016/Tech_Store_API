using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

public class Supplier
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string ContactName { get; set; }
    public string ContactTitle { get; set; }
    public string Mobile { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public Guid CityId { get; set; }

    public City City { get; set; }
}

public class SupplierConfig : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> entity)
    {
        entity.ToTable("suppliers");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("company_name")
            .HasColumnType("nvarchar(100)");
        entity.HasIndex(e => e.CompanyName)
            .HasDatabaseName("supplier_company_name_unique_index")
            .IsUnique();

        entity.Property(e => e.CompanyAddress)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("company_address")
            .HasColumnType("nvarchar(500)");

        entity.Property(e => e.ContactName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("contact_name")
            .HasColumnType("nvarchar(100)");

        entity.Property(e => e.ContactTitle)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("contact_title")
            .HasColumnType("nvarchar(100)");

        entity.Property(e => e.Mobile)
            .HasMaxLength(20)
            .HasColumnName("mobile")
            .HasColumnType("varchar(20)");

        entity.Property(e => e.Phone)
            .HasMaxLength(20)
            .HasColumnName("phone")
            .HasColumnType("varchar(20)");

        entity.Property(e => e.Fax)
            .HasMaxLength(20)
            .HasColumnName("fax")
            .HasColumnType("varchar(20)");

        entity.Property(e => e.CityId)
            .IsRequired()
            .HasColumnName("city_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.CityId, "cities_country_id_fk_index");
        entity.HasOne(supplier => supplier.City)
            .WithMany(country => country.Suppliers)
            .HasForeignKey(supplier => supplier.CityId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("cities_suppliers_fk");

    }
}

public class SupplierFaker : Faker<Supplier> {
    private short counter = 1;
    public SupplierFaker()
    {
        RuleFor(o => o.CompanyName, f => $"{counter++}_{f.Company.CompanyName()}");
        RuleFor(o => o.CompanyAddress, f => f.Address.StreetAddress());
        RuleFor(o => o.ContactName, f => f.Name.FullName(f.PickRandom<Bogus.DataSets.Name.Gender>()));
        RuleFor(o => o.ContactTitle, f => f.Name.JobTitle());
        RuleFor(o => o.Mobile, f => f.Phone.PhoneNumber().OrNull(f, 0.1f));
        RuleFor(o => o.Phone, f => f.Phone.PhoneNumber().OrNull(f, 0.4f));
        RuleFor(o => o.Fax, f => f.Phone.PhoneNumber().OrNull(f, 0.7f));
    }
}
