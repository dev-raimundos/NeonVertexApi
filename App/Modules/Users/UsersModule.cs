using CoeurApi.App.Modules.Users.Repositories;
using CoeurApi.App.Modules.Users.Services;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Users;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<CreateUserService>();
        services.AddScoped<GetUserByIdService>();
        services.AddScoped<UpdateUserService>();
        services.AddScoped<DeleteUserService>();

        return services;
    }
}