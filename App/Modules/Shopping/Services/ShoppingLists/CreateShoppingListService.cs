using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Services.ShoppingLists;

public class CreateShoppingListService(IShoppingListRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<ShoppingListResponse> ExecuteAsync(CreateShoppingListDto dto, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = ShoppingList.Create(dto.Name, ownerId);
        await repository.AddAsync(list, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ShoppingListResponse.FromEntity(list);
    }
}
