using CoeurApi.App.Modules.Shopping.Models;

namespace CoeurApi.App.Modules.Shopping.Repositories;

public interface IShoppingListRepository
{
    Task<(List<ShoppingList> Items, int TotalCount)> GetAllByOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ShoppingList?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListItem?> GetItemAsync(Guid listId, Guid itemId, CancellationToken cancellationToken = default);
    Task AddAsync(ShoppingList list, CancellationToken cancellationToken = default);
    Task AddItemAsync(ListItem item, CancellationToken cancellationToken = default);
    void Delete(ShoppingList list);
    void DeleteItem(ListItem item);
}
