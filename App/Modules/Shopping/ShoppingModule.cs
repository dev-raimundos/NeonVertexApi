using NeonVertexApi.App.Modules.Shopping.Repositories;
using NeonVertexApi.App.Modules.Shopping.Services;

namespace NeonVertexApi.App.Modules.Shopping;

public static class ShoppingModule
{
    public static IServiceCollection AddShoppingModule(this IServiceCollection services)
    {
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ShoppingListsService>();
        services.AddScoped<ProductsService>();

        return services;
    }
}
