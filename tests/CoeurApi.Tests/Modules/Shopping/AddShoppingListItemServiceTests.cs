using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Modules.Shopping.Services.ShoppingLists;
using CoeurApi.App.Shared.Interfaces;
using Moq;

namespace CoeurApi.Tests.Modules.Shopping;

public class AddShoppingListItemServiceTests
{
    private readonly Mock<IShoppingListRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private AddShoppingListItemService CreateService()
        => new(new GetOwnedShoppingListService(_repository.Object), _repository.Object, _unitOfWork.Object);

    [Fact]
    public async Task ExecuteAsync_DeveAdicionarItemNaListaDoDono()
    {
        var ownerId = Guid.NewGuid();
        var list = ShoppingList.Create("Mercado", ownerId);

        _repository.Setup(r => r.GetByIdWithItemsAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var service = CreateService();
        var dto = new AddListItemDto("Leite", 2, "un");

        var result = await service.ExecuteAsync(list.Id, dto, ownerId);

        Assert.Equal("Leite", result.Name);
        Assert.Equal(2, result.Quantity);
        _repository.Verify(r => r.AddItemAsync(It.IsAny<ListItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
