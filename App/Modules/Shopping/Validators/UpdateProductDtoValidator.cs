using FluentValidation;
using NeonVertexApi.App.Modules.Shopping.DTOs;

namespace NeonVertexApi.App.Modules.Shopping.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        this.ApplyProductFieldsRules();
    }
}
