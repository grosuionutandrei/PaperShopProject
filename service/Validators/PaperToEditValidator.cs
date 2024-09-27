
using FluentValidation;
using service.TransferModels.Request;
using utilities.ErrorMessages;

namespace service.Validators;

public class PaperToEditValidator:AbstractValidator<PaperToEditDto>
{
    public PaperToEditValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).NotNull().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorName));
        RuleFor(x => x.Discontinued).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.Discontinued));
        RuleFor(x => x.Price).GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.Price));
        RuleFor(x => x.Stock).GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.Discontinued));
    }
}