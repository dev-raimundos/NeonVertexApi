using CoeurApi.App.Modules.Shopping.Models;
using CoeurApi.App.Modules.Shopping.Repositories;
using CoeurApi.App.Shared.Exceptions;

namespace CoeurApi.App.Modules.Shopping.Services.ShoppingLists;

// Carrega uma lista garantindo que pertence ao dono informado — reaproveitado pelos
// demais services que precisam desse mesmo checkout de posse antes de agir sobre a lista.
public class GetOwnedShoppingListService(IShoppingListRepository repository)
{
    private const string ErrListNotFound = "Lista de compras não encontrada.";

    public async Task<ShoppingList> ExecuteAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var list = await repository.GetByIdWithItemsAsync(id, cancellationToken)
            ?? throw AppException.NotFound(ErrListNotFound);

        if (list.OwnerId != ownerId)
            throw AppException.Forbidden();

        return list;
    }
}
