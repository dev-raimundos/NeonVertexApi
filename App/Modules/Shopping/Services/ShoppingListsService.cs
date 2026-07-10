using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.DTOs;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Services;

public class ShoppingListsService(IShoppingListRepository repository, IUnitOfWork unitOfWork)
{
    private const string ErrListNotFound = "Lista de compras não encontrada.";
    private const string ErrItemNotFound = "Item não encontrado na lista.";

    public async Task<PagedResult<ShoppingListResponse>> GetAllAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (lists, totalCount) = await repository.GetAllByOwnerAsync(ownerId, page, pageSize, cancellationToken);
        return new PagedResult<ShoppingListResponse>(lists.Select(ShoppingListResponse.FromEntity).ToList(), page, pageSize, totalCount);
    }

    public async Task<ShoppingListResponse> GetByIdAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await GetOwnedListAsync(id, ownerId, cancellationToken);
        return ShoppingListResponse.FromEntity(list);
    }

    public async Task<ShoppingListResponse> CreateAsync(CreateShoppingListDto dto, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = ShoppingList.Create(dto.Name, ownerId);
        await repository.AddAsync(list, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ShoppingListResponse.FromEntity(list);
    }

    public async Task<ShoppingListResponse> UpdateAsync(Guid id, UpdateShoppingListDto dto, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await GetOwnedListAsync(id, ownerId, cancellationToken);

        list.Rename(dto.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ShoppingListResponse.FromEntity(list);
    }

    public async Task DeleteAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await GetOwnedListAsync(id, ownerId, cancellationToken);

        repository.Delete(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ListItemResponse> AddItemAsync(Guid listId, AddListItemDto dto, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await GetOwnedListAsync(listId, ownerId, cancellationToken);

        var item = ListItem.Create(listId, dto.Name, dto.Quantity, dto.Unit, dto.ProductId);
        await repository.AddItemAsync(item, cancellationToken);
        list.Touch();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ListItemResponse.FromEntity(item);
    }

    public async Task<ListItemResponse> UpdateItemAsync(Guid listId, Guid itemId, UpdateListItemDto dto, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await GetOwnedListAsync(listId, ownerId, cancellationToken);

        var item = await repository.GetItemAsync(listId, itemId, cancellationToken)
            ?? throw AppException.NotFound(ErrItemNotFound);

        if (dto.IsChecked is true) item.Check();
        else if (dto.IsChecked is false) item.Uncheck();

        if (dto.Name is not null || dto.Quantity is not null || dto.Unit is not null)
            item.UpdateDetails(dto.Name ?? item.Name, dto.Quantity ?? item.Quantity, dto.Unit ?? item.Unit);

        list.Touch();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ListItemResponse.FromEntity(item);
    }

    public async Task RemoveItemAsync(Guid listId, Guid itemId, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await GetOwnedListAsync(listId, ownerId, cancellationToken);

        var item = await repository.GetItemAsync(listId, itemId, cancellationToken)
            ?? throw AppException.NotFound(ErrItemNotFound);

        repository.DeleteItem(item);
        list.Touch();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<ShoppingList> GetOwnedListAsync(Guid id, Guid ownerId, CancellationToken cancellationToken)
    {
        var list = await repository.GetByIdWithItemsAsync(id, cancellationToken)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        return list;
    }
}
