using infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace api.TransferModels.Response;

public class DeletePropertyResponse
{
    private bool isDeleted { get; set; }
    private EditPaperPropertyDto _deleteDto;

    public DeletePropertyResponse(bool val, EditPaperPropertyDto deletePaperPropertyDto)
    {
        isDeleted = val;
        _deleteDto = deletePaperPropertyDto;
    }

    public ActionResult ConstructDeleteResponse(string message)
    {
        if (isDeleted)
        {
            return new OkObjectResult(_deleteDto);
        }

        return new NotFoundObjectResult(new { Error = message, Data = _deleteDto });
    }
}