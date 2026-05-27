using System.Security.Claims;
using NeonVertexApi.App.Shared.Interfaces;

namespace NeonVertexApi.App.Core.Authentication;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public Guid Id => Guid.Parse(User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
    public string Email => User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    public string Name => User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}