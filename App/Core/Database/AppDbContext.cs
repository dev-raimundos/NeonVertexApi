using Microsoft.EntityFrameworkCore;
using NeonVertexApi.App.Modules.Shopping.Models;
using NeonVertexApi.App.Modules.Users.Models;

namespace NeonVertexApi.App.Core.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ListItem> ListItems => Set<ListItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}