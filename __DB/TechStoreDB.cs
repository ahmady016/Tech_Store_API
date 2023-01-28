using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using TechStoreApi.Entities;

namespace TechStoreApi.DB;

public partial class TechStoreDB: IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public TechStoreDB() {}
    public TechStoreDB(DbContextOptions<TechStoreDB> options) : base(options) {}

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

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
    public DbSet<ContactUsMessage> ContactUsMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
