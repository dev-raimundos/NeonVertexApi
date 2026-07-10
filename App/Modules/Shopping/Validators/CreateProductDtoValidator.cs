using FluentValidation;
using NeonVertexApi.App.Modules.Shopping.DTOs;

namespace NeonVertexApi.App.Modules.Shopping.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        this.ApplyProductFieldsRules();
    }
}
