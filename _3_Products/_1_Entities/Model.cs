﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

using TechStoreApi.DB.Common;

namespace TechStoreApi.Entities;

public class Model : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string PhotoName { get; set; }
    public string ThumbName { get; set; }
    public ProductCategories CategoryId { get; set; }
    public Guid ProductId { get; set; }
    public Guid BrandId { get; set; }
    public long RatingCount { get; set; } = 0;
    public double RatingAverage { get; set; } = 0.0;

    public Product Product { get; set; }
    public Brand Brand { get; set; }
    public Stock Stock { get; set; }
    public virtual ICollection<PurchaseItem> PurchasesItems { get; set; } = new HashSet<PurchaseItem>();
    public virtual ICollection<SaleItem> SalesItems { get; set; } = new HashSet<SaleItem>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public virtual ICollection<CustomerFavoriteModel> CustomersFavoritesModels { get; set; } = new HashSet<CustomerFavoriteModel>();
    public virtual ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}

public class ModelConfig : EntityConfig<Model>
{
    public override void Configure(EntityTypeBuilder<Model> entity)
    {
        entity.ToTable("models");
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

        entity.Property(e => e.PhotoName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("photo_url")
            .HasColumnType("varchar(50)");

        entity.Property(e => e.ThumbName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("thumb_url")
            .HasColumnType("varchar(50)");

        entity.Property(e => e.CategoryId)
            .IsRequired()
            .HasColumnName("category_id")
            .HasColumnType("tinyint");

        entity.Property(e => e.RatingCount)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("rating_count")
            .HasColumnType("bigint");

        entity.Property(e => e.RatingAverage)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnName("rating_average")
            .HasColumnType("float");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("model_title_unique_index")
            .IsUnique();

        entity.Property(e => e.ProductId)
            .HasColumnName("product_id")
            .HasColumnType("uniqueidentifier");

        entity.Property(e => e.BrandId)
            .HasColumnName("brand_id")
            .HasColumnType("uniqueidentifier");

        entity.HasIndex(e => e.ProductId, "models_product_id_fk_index");
        entity.HasOne(model => model.Product)
            .WithMany(product => product.Models)
            .HasForeignKey(model => model.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("products_models_fk");

        entity.HasIndex(e => e.BrandId, "models_brand_id_fk_index");
        entity.HasOne(model => model.Brand)
            .WithMany(brand => brand.Models)
            .HasForeignKey(model => model.BrandId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("brands_models_fk");

        entity.HasOne(model => model.Stock)
            .WithOne(stock => stock.Model)
            .HasForeignKey<Stock>(stock => stock.ModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("models_stocks_fk");

    }
}

public class ModelFaker : Faker<Model> {
    private short counter = 1;
    public ModelFaker()
    {
        RuleFor(o => o.Title, f => $"{counter++}_{f.Commerce.ProductName()}");
        RuleFor(o => o.Description, f => f.Commerce.ProductDescription());
        RuleFor(o => o.PhotoName, f => f.Image.PicsumUrl());
        RuleFor(o => o.ThumbName, f => f.Image.PicsumUrl());
        RuleFor(o => o.CategoryId, f => f.PickRandom<ProductCategories>());
        RuleFor(o => o.RatingCount, f => f.Random.Long(10,10000));
        RuleFor(o => o.RatingAverage, f => f.Random.Double(1,5));
    }
}
