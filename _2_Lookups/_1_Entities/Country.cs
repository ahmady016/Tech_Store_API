using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

namespace TechStoreApi.Entities;

public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new HashSet<City>();
    public virtual ICollection<Brand> Brands { get; set; } = new HashSet<Brand>();
}

public class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> entity)
    {
        entity.ToTable("countries");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name")
            .HasColumnType("nvarchar(100)");

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("country_name_unique_index")
            .IsUnique();

    }
}

public class CountryFaker : Faker<Country> {
    private short counter = 1;
    public CountryFaker()
    {
        RuleFor(o => o.Name, f => $"{counter++}_{f.Address.Country()}");
    }
}
