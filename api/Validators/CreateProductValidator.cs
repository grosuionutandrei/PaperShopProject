using api.TransferModels;
using FluentValidation;
using utilities.ErrorMessages;

namespace api.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Discontinued).NotNull();
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorName));
        RuleFor(x => x.Price).GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.Price));
        RuleFor(x => x.Stock).GreaterThan(-1).WithMessage(ErrorMessages.GetMessage(ErrorCode.Stock));
    }
}