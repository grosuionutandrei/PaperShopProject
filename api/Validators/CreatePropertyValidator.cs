using api.TransferModels;
using FluentValidation;

namespace api.Validators;

public class CreatePropertyValidator : AbstractValidator<CreatePropertyDto>
{
    public CreatePropertyValidator()
    {
        RuleFor(property => property.PropertyName)
            .NotEmpty().WithMessage("Property name is required");
    }
}