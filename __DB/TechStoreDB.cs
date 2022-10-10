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
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UsersRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }

    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseItem> PurchasesItems { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SalesItems { get; set; }
    public DbSet<Stock> Stocks { get; set; }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reply> Replies { get; set; }

    public DbSet<CustomerFavoriteModel> CustomersFavoritesModels { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserRoleConfig());

        modelBuilder.ApplyConfiguration(new ProductConfig());
        modelBuilder.ApplyConfiguration(new BrandConfig());
        modelBuilder.ApplyConfiguration(new ModelConfig());

        modelBuilder.ApplyConfiguration(new PurchaseConfig());
        modelBuilder.ApplyConfiguration(new PurchaseItemConfig());
        modelBuilder.ApplyConfiguration(new SaleConfig());
        modelBuilder.ApplyConfiguration(new SaleItemConfig());
        modelBuilder.ApplyConfiguration(new StockConfig());

        modelBuilder.ApplyConfiguration(new CommentConfig());
        modelBuilder.ApplyConfiguration(new ReplyConfig());

        modelBuilder.ApplyConfiguration(new CustomerFavoriteModelConfig());
        modelBuilder.ApplyConfiguration(new RatingConfig());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
