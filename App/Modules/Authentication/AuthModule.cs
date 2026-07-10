using CoeurApi.App.Modules.Authentication.Services;

namespace CoeurApi.App.Modules.Authentication;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services)
    {
        services.AddScoped<LoginService>();
        return services;
    }
}
