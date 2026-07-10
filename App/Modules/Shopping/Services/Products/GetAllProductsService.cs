using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.DTOs;

namespace CoeurApi.App.Modules.Shopping.Services.Products;

public class GetAllProductsService(IProductRepository repository)
{
    public async Task<PagedResult<ProductResponse>> ExecuteAsync(string? category, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (products, totalCount) = await repository.GetAllAsync(category, page, pageSize, cancellationToken);
        return new PagedResult<ProductResponse>(products.Select(ProductResponse.FromEntity).ToList(), page, pageSize, totalCount);
    }
}
