
using api.TransferModels;
using api.TransferModels.Response;
using infrastructure.ErrorMessages;
using Microsoft.AspNetCore.Mvc;
using service.Paper;
using utilities.ErrorMessages;

namespace api.Controllers;


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
    public ActionResult GetPaper([FromRoute] int pageNumber,[FromQuery] string searchTerm,int pageItems,string orderParam,string filterParam,int propertyId)
    {
         var paperObjects = _paperService.GetPaperWithQuerries(pageNumber, searchTerm,pageItems,orderParam,filterParam,propertyId);
        return Ok(paperObjects);
    }
    
    //edit paper product

    [HttpGet]
    [Route("api/papers/details/{paperId}")]
    public Task<ActionResult> EditPaper([FromBody]  )
    {
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
    public async Task<ActionResult>  CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
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
    public async Task<ActionResult> EditProperty([FromBody] EditPaperPropertyDto editPaperPropertyDto )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var editedProperty = _paperService.EditPaperProperty(editPaperPropertyDto.PropertyId,editPaperPropertyDto.PropName);

        return Ok(editedProperty);

    } 
    //delete paper property
    [HttpPatch]
         [Route("/api/admin/deletePaperPropriety")]
         public async Task<ActionResult> DeleteProperty([FromBody] EditPaperPropertyDto editPaperPropertyDto )
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             var deletedProperty = await _paperService.DeletePaperProperty(editPaperPropertyDto.PropertyId,editPaperPropertyDto.PropName!);
             var  deleteResponse = new DeletePropertyResponse(deletedProperty,editPaperPropertyDto);
             return deleteResponse.ConstructDeleteResponse(ErrorMessages.GetMessage(ErrorCode.PropertyNotFound));
     
         }
   












}