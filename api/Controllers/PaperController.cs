using System.Runtime.InteropServices.JavaScript;
using api.TransferModels;
using api.TransferModels.Response;
using infrastructure.QuerryModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using service.Paper;
using utilities.ErrorMessages;

namespace api.Controllers;

[ApiController]
public class PaperController : ControllerBase
{
    private readonly ILogger<PaperController> _logger;
    private readonly IPaperService _paperService;

    public PaperController(IPaperService service, ILogger<PaperController> paperLogger)
    {
        _logger = paperLogger;
        _paperService = service;
    }

    //get paper products per pages
    [HttpGet]
    [Route("/api/papers/{pageNumber}")]
    public ActionResult GetPaper([FromRoute] int pageNumber,[FromQuery] PaperQueryDto paperQuerry)
    {
        Console.WriteLine("ana are mere");
        var request = new PaperQueryDto{PageNumber = pageNumber,PageItems = paperQuerry.PageItems};
        var paperObjects =
            _paperService.GetPaperWithQuerries( request.PageNumber,request.PageItems);
        return Ok(paperObjects);
    }


    //edit paper product

    [HttpPut]
    [Route("api/papers/edit/{paperId}")]
    public async Task<ActionResult<PaperToDisplay>> EditPaper([FromBody] EditPaperDto editPaperDto)
    {
        var paperToEdit = new PaperToDisplay
        {
            Id = editPaperDto.Id,
            Price = editPaperDto.Price,
            Discontinued = editPaperDto.Discontinued,
            Stock = editPaperDto.Stock,
            Name = editPaperDto.Name
        };
        var editedPaper = await _paperService.EditPaper(paperToEdit);
        if (editedPaper)
        {
            return new OkObjectResult(editPaperDto);
        }

        return new NotFoundObjectResult(new
            { Error = ErrorMessages.GetMessage(ErrorCode.PropertyNotFound), Data = editPaperDto });
    }

    //TODO
    //get the details of the paper depending on the ui
    [HttpGet]
    [Route("/api/papers/details/{paperId}")]
    public async Task<ActionResult<IEnumerable<PaperProperties>>> GetPaperById(int paperId)
    {
        var paperDetails = await _paperService.GetPaperById(paperId);
        return Ok(paperDetails);
    }


    /// <summary>
    /// Delete the paper by id.
    /// </summary>
    /// <value>The paper's unique identifier.</value>
    [HttpDelete]
    [Route("/api/papers/delete/{paperId}")]
    public async Task<ActionResult<bool>> DeletePaper(int paperId)
    {
        var isDeleted = await _paperService.DeletePaperById(paperId);
        if (!isDeleted)
        {
            return NotFound(isDeleted);
        }

        return Ok(isDeleted);
    }


    //PAPER Properties
    [HttpGet]
    [Route("/api/papers/proprieties")]
    public ActionResult GetProprieties()
    {
        var paperProprieties = _paperService.GetPaperProprieties();
        return Ok(paperProprieties);
    }

    //create a new paper property
    [HttpPost]
    [Route("/api/admin/createPaper")]
    public async Task<ActionResult> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var propertyCreated = await _paperService.CreatePaperProperty(createPropertyDto.PropertyName);
        return Ok(propertyCreated);
    }

    //edit paperProperty
    [HttpPatch]
    [Route("/api/admin/editPaperPropriety")]
    public async Task<ActionResult> EditProperty([FromBody] EditPaperPropertyDto editPaperPropertyDto)
    {
        var propertyToEdit = new PaperProperties
        {
            PropId = editPaperPropertyDto.PropertyId,
            PropName = editPaperPropertyDto.PropName
        };
        var editedProperty = _paperService.EditPaperProperty(propertyToEdit);
        return Ok(editedProperty);
    }

    //delete paper property
    [HttpPatch]
    [Route("/api/admin/deletePaperPropriety")]
    public async Task<ActionResult> DeleteProperty([FromBody] EditPaperPropertyDto editPaperPropertyDto)
    {
        var deletedProperty =
            await _paperService.DeletePaperProperty(editPaperPropertyDto.PropertyId, editPaperPropertyDto.PropName!);
        var deleteResponse = new DeletePropertyResponse(deletedProperty, editPaperPropertyDto);
        return deleteResponse.ConstructDeleteResponse(ErrorMessages.GetMessage(ErrorCode.PropertyNotFound));
    }
    
}