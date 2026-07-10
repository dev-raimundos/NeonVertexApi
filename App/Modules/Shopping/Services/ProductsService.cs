using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.DTOs;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Services;

public class ProductsService(IProductRepository repository, IUnitOfWork unitOfWork)
{
    private const string ErrNotFound = "Produto não encontrado.";

    public async Task<PagedResult<ProductResponse>> GetAllAsync(string? category, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (products, totalCount) = await repository.GetAllAsync(category, page, pageSize, cancellationToken);
        return new PagedResult<ProductResponse>(products.Select(ProductResponse.FromEntity).ToList(), page, pageSize, totalCount);
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound(ErrNotFound);

        return ProductResponse.FromEntity(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = Product.Create(dto.Name, dto.Category, dto.ImageUrl);
        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductResponse.FromEntity(product);
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound(ErrNotFound);

        product.Update(dto.Name, dto.Category, dto.ImageUrl);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductResponse.FromEntity(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound(ErrNotFound);

        repository.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
