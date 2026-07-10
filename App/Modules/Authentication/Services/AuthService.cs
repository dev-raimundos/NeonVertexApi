using CoeurApi.App.Core.Authentication;
using CoeurApi.App.Modules.Authentication.DTOs;
using CoeurApi.App.Modules.Users.DTOs;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Authentication.Services;

public class AuthService(IUsersRepository repository, TokenService tokenService, IUnitOfWork unitOfWork)
{
    private const string ErrInvalidCredentials = "Credenciais inválidas.";
    private const string ErrAccountLocked = "Conta bloqueada temporariamente. Tente novamente em alguns minutos.";
    private const string ErrAccountInactive = "Conta desativada.";

    // Hash BCrypt válido sem usuário correspondente — verificado mesmo quando o e-mail não
    // existe, pra manter o tempo de resposta constante e evitar enumeração via timing.
    private const string DummyHash = "$2a$11$CwTycUXWue0Thq9StjUM0uJ8vY.SEmR5AZlSZDPGGStLL55E1Wei.";

    public async Task<(AuthResponse Response, string Token)> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByEmailAsync(dto.Email, cancellationToken);

        bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user?.PasswordHash ?? DummyHash);

        if (user is null)
            throw AppException.Unauthorized(ErrInvalidCredentials);

        if (!user.IsActive)
            throw AppException.Forbidden(ErrAccountInactive);

        if (user.IsLocked)
            throw AppException.TooManyRequests(ErrAccountLocked);

        if (!passwordValid)
        {
            user.RecordFailedLogin();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw AppException.Unauthorized(ErrInvalidCredentials);
        }

        user.RecordLogin();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = tokenService.Generate(user);
        var response = new AuthResponse(UserResponse.FromEntity(user));

        return (response, token);
    }
}
