using Microsoft.EntityFrameworkCore;

namespace NeonVertexApi.src.Core.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User>
}
