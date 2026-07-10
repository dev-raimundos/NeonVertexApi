using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Modules.Shopping.Services;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;
using Moq;

namespace CoeurApi.Tests.Modules.Shopping;

public class ProductsServiceTests
{
    private readonly Mock<IProductRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private ProductsService CreateService() => new(_repository.Object, _unitOfWork.Object);

    [Fact]
    public async Task GetAllAsync_DeveRetornarPagedResultComTotalCount()
    {
        var products = new List<Product> { Product.Create("Arroz", "Mercearia") };
        _repository.Setup(r => r.GetAllAsync(null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 1));

        var service = CreateService();
        var result = await service.GetAllAsync(null, 1, 20);

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
    }

    [Fact]
    public async Task GetByIdAsync_ProdutoNaoEncontrado_DeveLancarNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = CreateService();

        var ex = await Assert.ThrowsAsync<AppException>(() => service.GetByIdAsync(Guid.NewGuid()));

        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_DeveCriarProdutoESalvar()
    {
        var service = CreateService();
        var dto = new CreateProductDto("Feijão", "Mercearia");

        var result = await service.CreateAsync(dto);

        Assert.Equal("Feijão", result.Name);
        _repository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
