using api.TransferModels;
using FluentValidation;
using utilities.ErrorMessages;

namespace api.Validators;

public class PaperQueryDtoValidator : AbstractValidator<PaperPaginationQueryDto>
{
    public PaperQueryDtoValidator()
    {
        RuleFor(x => x.PageNumber).NotNull().GreaterThan(-1)
            .WithMessage(ErrorMessages.GetMessage(ErrorCode.PageNumber));
        RuleFor(x => x.PageItems).NotEmpty().NotNull().GreaterThan(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCode.PageItems));
    }
}