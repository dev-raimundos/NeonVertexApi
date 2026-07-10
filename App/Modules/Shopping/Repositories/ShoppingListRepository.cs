using Microsoft.EntityFrameworkCore;
using CoeurApi.App.Core.Database;
using CoeurApi.App.Modules.Shopping.Models;

namespace CoeurApi.App.Modules.Shopping.Repositories;

public class ShoppingListRepository(AppDbContext context) : IShoppingListRepository
{
    public async Task<(List<ShoppingList> Items, int TotalCount)> GetAllByOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.ShoppingLists
            .Where(l => l.OwnerId == ownerId)
            .OrderByDescending(l => l.UpdatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<ShoppingList?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.ShoppingLists
            .Include(l => l.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<ListItem?> GetItemAsync(Guid listId, Guid itemId, CancellationToken cancellationToken = default)
        => await context.ListItems
            .FirstOrDefaultAsync(i => i.ShoppingListId == listId && i.Id == itemId, cancellationToken);

    public async Task AddAsync(ShoppingList list, CancellationToken cancellationToken = default)
        => await context.ShoppingLists.AddAsync(list, cancellationToken);

    public async Task AddItemAsync(ListItem item, CancellationToken cancellationToken = default)
        => await context.ListItems.AddAsync(item, cancellationToken);

    public void Delete(ShoppingList list)
        => context.ShoppingLists.Remove(list);

    public void DeleteItem(ListItem item)
        => context.ListItems.Remove(item);
}
