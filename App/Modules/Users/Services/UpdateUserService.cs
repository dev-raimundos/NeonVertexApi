using CoeurApi.App.Modules.Users.DTOs;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Users.Services;

public class UpdateUserService(IUsersRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
{
    private const string ErrNotFound = "Usuário não encontrado.";

    public async Task<UserResponse> ExecuteAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (id != currentUser.Id && !currentUser.IsAdmin)
            throw HttpException.Forbidden();

        var user = await repository.GetByIdAsync(id, cancellationToken) ?? throw HttpException.NotFound(ErrNotFound);
        user.UpdateProfile(dto.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }
}
