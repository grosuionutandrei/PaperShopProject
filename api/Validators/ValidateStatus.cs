using FluentValidation;
using utilities.ErrorMessages;
using utilities.OrderStatus;

namespace api.Validators;

public class ValidateStatus:AbstractValidator<string>
{
    public ValidateStatus()
    {
        RuleFor(x => x).Must(IsValidStatus).WithMessage(ErrorMessages.GetMessage(ErrorCode.StatusInvalid));
    }

    private static bool IsValidStatus(string status)
    {
        return Enum.GetValues(typeof(OrderStatus)).Cast<object?>().Any(value => value!.ToString()!.Equals(status, StringComparison.OrdinalIgnoreCase));
    }
}