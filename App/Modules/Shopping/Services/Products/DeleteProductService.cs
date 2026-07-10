using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Services.Products;

public class DeleteProductService(IProductRepository repository, IUnitOfWork unitOfWork)
{
    private const string ErrNotFound = "Produto não encontrado.";

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw HttpException.NotFound(ErrNotFound);

        repository.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
