using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Services.Products;

public class CreateProductService(IProductRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<ProductResponse> ExecuteAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = Product.Create(dto.Name, dto.Category, dto.ImageUrl);
        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductResponse.FromEntity(product);
    }
}
