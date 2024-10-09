using api.TransferModels;
using FluentValidation;
using service.Paper;
using utilities.ErrorMessages;

namespace api.Validators;

public class ValidateRemovePropertyFromPaper : AbstractValidator<RemovePropertyDto>
{
    private IPaperService _paperService;

    public ValidateRemovePropertyFromPaper(IPaperService paperService)
    {
        _paperService = paperService;
        RuleFor(x => x.paperId).NotEmpty().NotNull().Must(PaperExist)
            .WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
        RuleFor(x => x.propertyId).NotEmpty().NotNull().Must(PropertyExist)
            .WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
    }

    private bool PaperExist(int paperId)
    {
        return  _paperService.PaperExistsAsync(paperId);
    }

    private bool PropertyExist(int propertyId)
    {
        return  _paperService.PropertyExistsAsync(propertyId);
    }
}