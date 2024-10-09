using FluentValidation;
using utilities.ErrorMessages;

namespace api.Validators;

public class ValidateId:AbstractValidator<int>
{
    public ValidateId()
    {
        RuleFor(x => x).NotNull().NotEmpty().GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
    }
}