using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

using TechStoreApi.DB.Common;

namespace TechStoreApi.Entities;

public class Brand : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LogoName { get; set; }
    public Guid CountryId { get; set; }

    public Country Country { get; set; }
    public virtual ICollection<Model> Models { get; set; } = new HashSet<Model>();
}

public class BrandConfig : EntityConfig<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> entity)
    {
        entity.ToTable("brands");
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("title")
            .HasColumnType("varchar(100)");
        entity.HasIndex(e => e.Title)
            .HasDatabaseName("brand_title_unique_index")
            .IsUnique();

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(400)
            .HasColumnName("description")
            .HasColumnType("varchar(400)");

        entity.Property(e => e.LogoName)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("logo_name")
            .HasColumnType("varchar(500)");

        entity.Property(e => e.CountryId)
            .HasColumnName("country_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.CountryId, "brands_country_id_fk_index");
        entity.HasOne(brand => brand.Country)
            .WithMany(country => country.Brands)
            .HasForeignKey(brand => brand.CountryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("brands_countries_fk");

    }
}

public class BrandFaker : Faker<Brand> {
    private short counter = 1;
    public BrandFaker()
    {
        RuleFor(o => o.Title, f => $"{counter++}_{f.Company.CompanyName()}");
        RuleFor(o => o.Description, f => f.Commerce.ProductDescription());
        RuleFor(o => o.LogoName, f => f.Image.PicsumUrl());
    }
}
