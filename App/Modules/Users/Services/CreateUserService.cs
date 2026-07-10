using CoeurApi.App.Modules.Users.DTOs;
using CoeurApi.App.Modules.Users.Models;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Users.Services;

public class CreateUserService(IUsersRepository repository, IUnitOfWork unitOfWork)
{
    private const string ErrEmailInUse = "Email já está em uso.";

    public async Task<UserResponse> ExecuteAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsByEmailAsync(dto.Email, cancellationToken))
            throw AppException.Conflict(ErrEmailInUse);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = User.Create(dto.Name, dto.Email, passwordHash);

        await repository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }
}
