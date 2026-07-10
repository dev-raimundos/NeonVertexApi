using Microsoft.EntityFrameworkCore;
using CoeurApi.App.Core.Database;
using CoeurApi.App.Modules.Users.Models;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Users.Repositories;

public class UsersRepository(AppDbContext context) : IUsersRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await context.Users.AddAsync(user, cancellationToken);

    public void Delete(User user)
        => context.Users.Remove(user);
}
