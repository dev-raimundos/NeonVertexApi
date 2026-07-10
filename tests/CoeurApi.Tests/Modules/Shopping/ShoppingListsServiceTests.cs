using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Modules.Shopping.Services;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;
using Moq;

namespace CoeurApi.Tests.Modules.Shopping;

public class ShoppingListsServiceTests
{
    private readonly Mock<IShoppingListRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private ShoppingListsService CreateService() => new(_repository.Object, _unitOfWork.Object);

    [Fact]
    public async Task GetByIdAsync_ListaDeOutroDono_DeveLancarForbidden()
    {
        var list = ShoppingList.Create("Mercado", Guid.NewGuid());
        _repository.Setup(r => r.GetByIdWithItemsAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var service = CreateService();

        var ex = await Assert.ThrowsAsync<AppException>(() => service.GetByIdAsync(list.Id, Guid.NewGuid()));

        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task GetByIdAsync_ListaInexistente_DeveLancarNotFound()
    {
        _repository.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingList?)null);

        var service = CreateService();

        var ex = await Assert.ThrowsAsync<AppException>(() => service.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task AddItemAsync_DeveAdicionarItemNaListaDoDono()
    {
        var ownerId = Guid.NewGuid();
        var list = ShoppingList.Create("Mercado", ownerId);

        _repository.Setup(r => r.GetByIdWithItemsAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var service = CreateService();
        var dto = new AddListItemDto("Leite", 2, "un");

        var result = await service.AddItemAsync(list.Id, dto, ownerId);

        Assert.Equal("Leite", result.Name);
        Assert.Equal(2, result.Quantity);
        _repository.Verify(r => r.AddItemAsync(It.IsAny<ListItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_ItemNaoEncontrado_DeveLancarNotFound()
    {
        var ownerId = Guid.NewGuid();
        var list = ShoppingList.Create("Mercado", ownerId);

        _repository.Setup(r => r.GetByIdWithItemsAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _repository.Setup(r => r.GetItemAsync(list.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ListItem?)null);

        var service = CreateService();
        var dto = new UpdateListItemDto(IsChecked: true);

        var ex = await Assert.ThrowsAsync<AppException>(() => service.UpdateItemAsync(list.Id, Guid.NewGuid(), dto, ownerId));

        Assert.Equal(404, ex.StatusCode);
    }
}
