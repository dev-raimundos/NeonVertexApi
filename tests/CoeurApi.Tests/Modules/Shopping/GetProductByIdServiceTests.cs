using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Modules.Shopping.Services.Products;
using CoeurApi.App.Shared.Exceptions;
using Moq;

namespace CoeurApi.Tests.Modules.Shopping;

public class GetProductByIdServiceTests
{
    private readonly Mock<IProductRepository> _repository = new();

    private GetProductByIdService CreateService() => new(_repository.Object);

    [Fact]
    public async Task ExecuteAsync_ProdutoNaoEncontrado_DeveLancarNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = CreateService();

        var ex = await Assert.ThrowsAsync<HttpException>(() => service.ExecuteAsync(Guid.NewGuid()));

        Assert.Equal(404, ex.StatusCode);
    }
}
