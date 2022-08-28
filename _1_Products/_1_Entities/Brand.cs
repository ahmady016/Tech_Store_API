using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DB.Common;

namespace Entities;

public class Brand : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }

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

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(400)
            .HasColumnName("description")
            .HasColumnType("varchar(400)");

        entity.Property(e => e.LogoUrl)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("logo_url")
            .HasColumnType("varchar(500)");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("brand_title_unique_index")
            .IsUnique();

    }
}
