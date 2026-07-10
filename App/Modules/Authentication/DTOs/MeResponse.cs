namespace CoeurApi.App.Modules.Authentication.DTOs;

public record MeResponse(
    Guid Id,
    string Name,
    string Email
);
