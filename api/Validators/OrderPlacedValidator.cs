using api.TransferModels;
using FluentValidation;
using utilities.ErrorMessages;

namespace api.Validators;

public class OrderPlacedValidator:AbstractValidator<OrderPlacedDto>
{
    public OrderPlacedValidator()
    {
        RuleFor(x => x.CustomerId).NotNull().NotEmpty().GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
        RuleFor(x => x.OrderPlacedProducts).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.NoProducts));
    }
}