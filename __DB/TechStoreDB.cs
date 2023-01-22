using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace TechStoreApi.DB;

public partial class TechStoreDB : DbContext
{
    public TechStoreDB() {}
    public TechStoreDB(DbContextOptions<TechStoreDB> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
