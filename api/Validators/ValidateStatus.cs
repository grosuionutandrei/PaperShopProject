using api.TransferModels;
using FluentValidation;
using service.Orders;
using utilities.ErrorMessages;
namespace api.Validators;

public class ValidateStatus:AbstractValidator<Status>
{
    IOrderService _orderService; 
    public ValidateStatus(IOrderService orderService)
    {
        _orderService = orderService;
        RuleFor(x => x).Must(IsValidStatus).WithMessage(ErrorMessages.GetMessage(ErrorCode.StatusInvalid));
    }


    private bool IsValidStatus(Status status)
    {
        if (string.IsNullOrWhiteSpace(status.status))
        {
            Console.WriteLine("Why null");
            return false;
        }

        Console.WriteLine(status + " From validator");
        return _orderService.ValidateStatus(status.status);
    }
}