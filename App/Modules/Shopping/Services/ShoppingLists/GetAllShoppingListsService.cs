using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.DTOs;

namespace CoeurApi.App.Modules.Shopping.Services.ShoppingLists;

public class GetAllShoppingListsService(IShoppingListRepository repository)
{
    public async Task<PagedResult<ShoppingListResponse>> ExecuteAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (lists, totalCount) = await repository.GetAllByOwnerAsync(ownerId, page, pageSize, cancellationToken);
        return new PagedResult<ShoppingListResponse>(lists.Select(ShoppingListResponse.FromEntity).ToList(), page, pageSize, totalCount);
    }
}
