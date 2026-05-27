using NeonVertexApi.App.Modules.Users.DTOs;

namespace NeonVertexApi.App.Modules.Authentication.DTOs;

public record AuthResponse(
    UserResponse User
);
