using api.TransferModels;
using FluentValidation;
using service.Paper;
using utilities.ErrorMessages;

namespace api.Validators;

public class EditPaperValidator : AbstractValidator<EditPaperDto>
{
    private readonly IPaperService _paperService;

    public EditPaperValidator(IPaperService paperService)
    {
        _paperService = paperService;
        RuleFor(x => x.Id).Must(ArePaperEqual).WithMessage(ErrorMessages.GetMessage(ErrorCode.IdNotEqual));
        RuleFor(x => x.Discontinued).NotNull();
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorName));
        RuleFor(x => x.Price).GreaterThan(0).WithMessage(ErrorMessages.GetMessage(ErrorCode.Price));
        RuleFor(x => x.Stock).GreaterThan(-1).WithMessage(ErrorMessages.GetMessage(ErrorCode.Stock));
    }

    private bool ArePaperEqual(int id)
    {
        return _paperService.ArePaperObjectsEqual(id).Result;
    }
}