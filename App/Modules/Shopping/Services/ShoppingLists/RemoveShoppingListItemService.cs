using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Services.ShoppingLists;

public class RemoveShoppingListItemService(GetOwnedShoppingListService getOwnedList, IShoppingListRepository repository, IUnitOfWork unitOfWork)
{
    private const string ErrItemNotFound = "Item não encontrado na lista.";

    public async Task ExecuteAsync(Guid listId, Guid itemId, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await getOwnedList.ExecuteAsync(listId, ownerId, cancellationToken);

        var item = await repository.GetItemAsync(listId, itemId, cancellationToken)
            ?? throw HttpException.NotFound(ErrItemNotFound);

        repository.DeleteItem(item);
        list.Touch();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
