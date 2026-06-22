using Microsoft.EntityFrameworkCore;
using NeonVertexApi.App.Core.Database;
using NeonVertexApi.App.Modules.Shopping.Models;

namespace NeonVertexApi.App.Modules.Shopping.Repositories;

public class ShoppingListRepository(AppDbContext context) : IShoppingListRepository
{
    public async Task<List<ShoppingList>> GetAllByOwnerAsync(Guid ownerId)
        => await context.ShoppingLists
            .Where(l => l.OwnerId == ownerId)
            .OrderByDescending(l => l.UpdatedAt)
            .ToListAsync();

    public async Task<ShoppingList?> GetByIdWithItemsAsync(Guid id)
        => await context.ShoppingLists
            .Include(l => l.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<ListItem?> GetItemAsync(Guid listId, Guid itemId)
        => await context.ListItems
            .FirstOrDefaultAsync(i => i.ShoppingListId == listId && i.Id == itemId);

    public async Task AddAsync(ShoppingList list)
        => await context.ShoppingLists.AddAsync(list);

    public async Task AddItemAsync(ListItem item)
        => await context.ListItems.AddAsync(item);

    public void Update(ShoppingList list)
        => context.ShoppingLists.Update(list);

    public void UpdateItem(ListItem item)
        => context.ListItems.Update(item);

    public void Delete(ShoppingList list)
        => context.ShoppingLists.Remove(list);

    public void DeleteItem(ListItem item)
        => context.ListItems.Remove(item);
}
