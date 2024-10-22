using Microsoft.AspNetCore.Mvc;

namespace api.TransferModels.Response;

public class DeletePropertyResponse
{
    private readonly EditPaperPropertyDto _deleteDto;

    public DeletePropertyResponse(bool val, EditPaperPropertyDto deletePaperPropertyDto)
    {
        isDeleted = val;
        _deleteDto = deletePaperPropertyDto;
    }

    private bool isDeleted { get; }

    public ActionResult ConstructDeleteResponse(string message)
    {
        if (isDeleted) return new OkObjectResult(_deleteDto);

        return new NotFoundObjectResult(new { Error = message, Data = _deleteDto });
    }
}