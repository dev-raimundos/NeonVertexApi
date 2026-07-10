using CoeurApi.App.Modules.Users.DTOs;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Users.Services;

public class GetUserByIdService(IUsersRepository repository, ICurrentUser currentUser)
{
    private const string ErrNotFound = "Usuário não encontrado.";

    public async Task<UserResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id != currentUser.Id && !currentUser.IsAdmin)
            throw AppException.Forbidden();

        var user = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound(ErrNotFound);

        return UserResponse.FromEntity(user);
    }
}
