using Microsoft.AspNetCore.Identity;
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

    public DbSet<User> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<UserRole> UsersRoles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserRoleConfig());
        modelBuilder.ApplyConfiguration(new ProductConfig());
        modelBuilder.ApplyConfiguration(new BrandConfig());
        modelBuilder.ApplyConfiguration(new ModelConfig());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
