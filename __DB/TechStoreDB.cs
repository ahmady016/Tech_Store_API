using Microsoft.EntityFrameworkCore;
using Entities;

namespace DB;

public partial class TechStoreDB : DbContext
{
    public TechStoreDB()
    {
    }
    public TechStoreDB(DbContextOptions<TechStoreDB> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfig());
        modelBuilder.ApplyConfiguration(new BrandConfig());
        modelBuilder.ApplyConfiguration(new ModelConfig());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
