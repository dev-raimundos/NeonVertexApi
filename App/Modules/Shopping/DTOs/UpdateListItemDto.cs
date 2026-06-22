namespace NeonVertexApi.App.Modules.Shopping.DTOs;

public record UpdateListItemDto(
    string? Name = null,
    int? Quantity = null,
    string? Unit = null,
    bool? IsChecked = null
);
