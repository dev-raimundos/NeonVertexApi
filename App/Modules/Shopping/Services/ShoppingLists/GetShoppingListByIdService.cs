using CoeurApi.App.Modules.Shopping.DTOs;

namespace CoeurApi.App.Modules.Shopping.Services.ShoppingLists;

public class GetShoppingListByIdService(GetOwnedShoppingListService getOwnedList)
{
    public async Task<ShoppingListResponse> ExecuteAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await getOwnedList.ExecuteAsync(id, ownerId, cancellationToken);
        return ShoppingListResponse.FromEntity(list);
    }
}
