using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.Exceptions;

namespace CoeurApi.App.Modules.Shopping.Services.Products;

public class GetProductByIdService(IProductRepository repository)
{
    private const string ErrNotFound = "Produto não encontrado.";

    public async Task<ProductResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound(ErrNotFound);

        return ProductResponse.FromEntity(product);
    }
}
