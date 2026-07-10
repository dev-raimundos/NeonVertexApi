using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Users.Services;

public class DeleteUserService(IUsersRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
{
    private const string ErrNotFound = "Usuário não encontrado.";

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id != currentUser.Id && !currentUser.IsAdmin)
            throw AppException.Forbidden();

        var user = await repository.GetByIdAsync(id, cancellationToken) ?? throw AppException.NotFound(ErrNotFound);

        repository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
