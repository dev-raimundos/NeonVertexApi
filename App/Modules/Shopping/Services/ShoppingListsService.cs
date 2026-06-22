using NeonVertexApi.App.Core.Database;
using NeonVertexApi.App.Modules.Shopping.DTOs;
using NeonVertexApi.App.Modules.Shopping.Models;
using NeonVertexApi.App.Modules.Shopping.Repositories;
using NeonVertexApi.App.Shared.Exceptions;

namespace NeonVertexApi.App.Modules.Shopping.Services;

public class ShoppingListsService(IShoppingListRepository repository, AppDbContext context)
{
    private const string ErrListNotFound = "Lista de compras não encontrada.";
    private const string ErrItemNotFound = "Item não encontrado na lista.";

    public async Task<List<ShoppingListResponse>> GetAllAsync(Guid ownerId)
    {
        var lists = await repository.GetAllByOwnerAsync(ownerId);
        return lists.Select(ShoppingListResponse.FromEntity).ToList();
    }

    public async Task<ShoppingListResponse> GetByIdAsync(Guid id, Guid ownerId)
    {
        var list = await repository.GetByIdWithItemsAsync(id)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        return ShoppingListResponse.FromEntity(list);
    }

    public async Task<ShoppingListResponse> CreateAsync(CreateShoppingListDto dto, Guid ownerId)
    {
        var list = ShoppingList.Create(dto.Name, ownerId);
        await repository.AddAsync(list);
        await context.SaveChangesAsync();

        return ShoppingListResponse.FromEntity(list);
    }

    public async Task<ShoppingListResponse> UpdateAsync(Guid id, UpdateShoppingListDto dto, Guid ownerId)
    {
        var list = await repository.GetByIdWithItemsAsync(id)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        list.Rename(dto.Name);
        await context.SaveChangesAsync();

        return ShoppingListResponse.FromEntity(list);
    }

    public async Task DeleteAsync(Guid id, Guid ownerId)
    {
        var list = await repository.GetByIdWithItemsAsync(id)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        repository.Delete(list);
        await context.SaveChangesAsync();
    }

    public async Task<ListItemResponse> AddItemAsync(Guid listId, AddListItemDto dto, Guid ownerId)
    {
        var list = await repository.GetByIdWithItemsAsync(listId)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        var item = ListItem.Create(listId, dto.Name, dto.Quantity, dto.Unit, dto.ProductId);
        await repository.AddItemAsync(item);
        await context.SaveChangesAsync();

        return ListItemResponse.FromEntity(item);
    }

    public async Task<ListItemResponse> UpdateItemAsync(Guid listId, Guid itemId, UpdateListItemDto dto, Guid ownerId)
    {
        var list = await repository.GetByIdWithItemsAsync(listId)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        var item = await repository.GetItemAsync(listId, itemId)
            ?? throw AppException.NotFound(ErrItemNotFound);

        if (dto.IsChecked is true) item.Check();
        else if (dto.IsChecked is false) item.Uncheck();

        if (dto.Name is not null || dto.Quantity is not null || dto.Unit is not null)
            item.UpdateDetails(dto.Name ?? item.Name, dto.Quantity ?? item.Quantity, dto.Unit ?? item.Unit);

        await context.SaveChangesAsync();

        return ListItemResponse.FromEntity(item);
    }

    public async Task RemoveItemAsync(Guid listId, Guid itemId, Guid ownerId)
    {
        var list = await repository.GetByIdWithItemsAsync(listId)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        var item = await repository.GetItemAsync(listId, itemId)
            ?? throw AppException.NotFound(ErrItemNotFound);

        repository.DeleteItem(item);
        await context.SaveChangesAsync();
    }
}
