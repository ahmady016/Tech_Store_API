using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

public class City
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CountryId { get; set; }

    public Country Country { get; set; }
    public virtual ICollection<Supplier> Suppliers { get; set; } = new HashSet<Supplier>();
}

public class CityConfig : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> entity)
    {
        entity.ToTable("cities");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name")
            .HasColumnType("nvarchar(100)");

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("city_name_unique_index")
            .IsUnique();

        entity.Property(e => e.CountryId)
            .HasColumnName("country_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.CountryId, "cities_country_id_fk_index");
        entity.HasOne(city => city.Country)
            .WithMany(country => country.Cities)
            .HasForeignKey(city => city.CountryId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("countries_cities_fk");

    }
}

public class CityFaker : Faker<City> {
    private short counter = 1;
    public CityFaker()
    {
        RuleFor(o => o.Name, f => $"{counter++}_{f.Address.City()}");
    }
}
