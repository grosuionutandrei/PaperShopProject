using api.TransferModels;
using FluentValidation;
using utilities.ErrorMessages;

namespace api.Validators;

public class EditPropertyValidator : AbstractValidator<EditPaperPropertyDto>
{
    public EditPropertyValidator()
    {
        RuleFor(x => x.PropertyId).GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));

        RuleFor(x => x.PropName).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorName));
    }
}