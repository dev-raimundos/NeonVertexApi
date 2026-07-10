using NeonVertexApi.App.Modules.Users.Models;

namespace NeonVertexApi.App.Shared.Interfaces;

public interface IUsersRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    void Delete(User user);
}