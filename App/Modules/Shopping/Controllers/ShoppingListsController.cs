using Microsoft.AspNetCore.Mvc;
using CoeurApi.App.Modules.Shopping.DTOs;
using CoeurApi.App.Modules.Shopping.Services;
using CoeurApi.App.Shared.DTOs;
using CoeurApi.App.Shared.Interfaces;

namespace CoeurApi.App.Modules.Shopping.Controllers;

[ApiController]
[Route("api/shopping-lists")]
public class ShoppingListsController(ShoppingListsService service, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ShoppingListResponse>>> GetAll(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = Pagination.Normalize(page, pageSize);
        var lists = await service.GetAllAsync(currentUser.Id, normalizedPage, normalizedPageSize, cancellationToken);
        return Ok(lists);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShoppingListResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var list = await service.GetByIdAsync(id, currentUser.Id, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingListResponse>> Create([FromBody] CreateShoppingListDto dto, CancellationToken cancellationToken)
    {
        var list = await service.CreateAsync(dto, currentUser.Id, cancellationToken);
        return Created($"api/shopping-lists/{list.Id}", list);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ShoppingListResponse>> Update(Guid id, [FromBody] UpdateShoppingListDto dto, CancellationToken cancellationToken)
    {
        var list = await service.UpdateAsync(id, dto, currentUser.Id, cancellationToken);
        return Ok(list);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, currentUser.Id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/items")]
    public async Task<ActionResult<ListItemResponse>> AddItem(Guid id, [FromBody] AddListItemDto dto, CancellationToken cancellationToken)
    {
        var item = await service.AddItemAsync(id, dto, currentUser.Id, cancellationToken);
        return Created($"api/shopping-lists/{id}/items/{item.Id}", item);
    }

    [HttpPatch("{id:guid}/items/{itemId:guid}")]
    public async Task<ActionResult<ListItemResponse>> UpdateItem(Guid id, Guid itemId, [FromBody] UpdateListItemDto dto, CancellationToken cancellationToken)
    {
        var item = await service.UpdateItemAsync(id, itemId, dto, currentUser.Id, cancellationToken);
        return Ok(item);
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    public async Task<ActionResult> RemoveItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        await service.RemoveItemAsync(id, itemId, currentUser.Id, cancellationToken);
        return NoContent();
    }
}
