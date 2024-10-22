using api.TransferModels;
using FluentValidation;
using utilities.ErrorMessages;

namespace api.Validators;

public class ValidateId : AbstractValidator<IdentificationDto>
{
    public ValidateId()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
    }
}