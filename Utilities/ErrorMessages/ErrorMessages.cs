namespace utilities.ErrorMessages;

public enum ErrorCode
{
    ErrorId,
    ErrorName,
    ErrorEmail,
    InvalidPassword,
    UserNotFound,
    UnauthorizedAccess,
    InternalServerError,
    PropertyNotFound,
    Discontinued,
    Price,
    Stock,
    IdNotEqual,
    StatusInvalid,
    NoProducts,
    PageNumber,
    PageItems
}


public static class ErrorMessages
{
    private static readonly Dictionary<ErrorCode, string> _errorMessages = new Dictionary<ErrorCode, string>
    {
        { ErrorCode.ErrorId, "Id is required" },
        { ErrorCode.IdNotEqual,"Wrong request entity is not present" },
        { ErrorCode.ErrorName, "Name is required" },
        { ErrorCode.ErrorEmail, "Email is invalid" },
        { ErrorCode.InvalidPassword, "The password is incorrect" },
        { ErrorCode.UserNotFound, "User not found" },
        { ErrorCode.UnauthorizedAccess, "Unauthorized access" },
        { ErrorCode.InternalServerError, "An internal server error occurred" },
        {ErrorCode.PropertyNotFound,"Property is not present"},
        {ErrorCode.Discontinued,"Discontinued value is required"},
        {ErrorCode.Price,"Price must be bigger than zero"},
        {ErrorCode.Stock,"Stock value can not be negative"},
        { ErrorCode.StatusInvalid ,"Status invalid"},
        { ErrorCode.NoProducts ,"Order is containing no products"},
        { ErrorCode.PageNumber,"Page number is invalid must be bigger or equal to zero" },
        { ErrorCode.PageItems ,"Page items value needs to be bigger than 0"}
        
    };
    
    public static string GetMessage(ErrorCode errorCode)
    {
        return _errorMessages.GetValueOrDefault(errorCode,"This error is undefined");
    }
}