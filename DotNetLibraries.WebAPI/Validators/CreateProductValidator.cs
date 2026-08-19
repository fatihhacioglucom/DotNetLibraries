using DotNetLibraries.FluentValidation;
using DotNetLibraries.WebAPI.Dtos;

namespace DotNetLibraries.WebAPI.Validators;

public sealed class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be a positive value.");
    }
}