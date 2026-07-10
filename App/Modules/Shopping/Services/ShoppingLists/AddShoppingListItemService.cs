using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Services.ShoppingLists;

public class AddShoppingListItemService(GetOwnedShoppingListService getOwnedList, IShoppingListRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<ListItemResponse> ExecuteAsync(Guid listId, AddListItemDto dto, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await getOwnedList.ExecuteAsync(listId, ownerId, cancellationToken);

        var item = ListItem.Create(listId, dto.Name, dto.Quantity, dto.Unit, dto.ProductId);
        await repository.AddItemAsync(item, cancellationToken);
        list.Touch();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ListItemResponse.FromEntity(item);
    }
}
