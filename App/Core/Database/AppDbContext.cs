using Microsoft.EntityFrameworkCore;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Users.Models;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Core.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
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