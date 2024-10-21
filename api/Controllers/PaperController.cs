using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
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
    public ActionResult GetPaper([FromRoute] int pageNumber, [FromQuery] PaperPaginationQueryDto paperPaginationQuerry)
    {
        var request = new PaperPaginationQueryDto
            { PageNumber = pageNumber, PageItems = paperPaginationQuerry.PageItems };
        var paperObjects =
            _paperService.GetPaperWithQuerries(request.PageNumber, request.PageItems);
        return Ok(paperObjects);
    }

    [HttpGet]
    [Route("/api/papers/filter")]
    public async Task<ActionResult<IEnumerable<PaperToDisplay>>> GetPaperByFilter(
        [FromQuery] PaperFilterDto paperFilterDto)
    {
        _logger.Log(LogLevel.Critical, JsonSerializer.Serialize(paperFilterDto));

        // Check if the paperFilterDto is null
        if (paperFilterDto == null)
        {
            return BadRequest("PaperFilterDto cannot be null.");
        }

        // Check if pagination is null
        if (paperFilterDto.pagination == null)
        {
            return BadRequest("Pagination cannot be null.");
        }

        // Check if priceRange is null
        if (paperFilterDto.priceRange == null)
        {
            return BadRequest("PriceRange cannot be null.");
        }

        var filterPapers = new PaperFilterQuery
        {
            searchFilter = paperFilterDto.searchFilter,
            pageNumber = paperFilterDto.pagination!.PageNumber,
            pageItems = paperFilterDto.pagination!.PageItems,
            priceRange = new PriceRange
            {
                minimumRange = paperFilterDto.priceRange!.minimumRange,
                maximumRange = paperFilterDto.priceRange!.maximumRange
            },
            paperPropertiesIds = paperFilterDto.GetParsedPaperPropertiesIds()
        };
        var result = await _paperService.GetPapersByFilter(filterPapers);
        return Ok(result);
    }


    [HttpGet]
    [Route("/api/papers/initialization/priceRange")]
    public async Task<ActionResult<PriceRange>> GetPriceRange()
    {
        var priceRange = await _paperService.GetPriceRange();
        return Ok(priceRange);
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


    [HttpPatch]
    [Route("api/papers/edit/{paperId}/properties/remove/{propertyId}")]
    public async Task<ActionResult<bool>> EditPaper([FromRoute] RemovePropertyDto removePropertyDto)
    {
        var result =
            await _paperService.RemovePropertyFromPaper(removePropertyDto.paperId, removePropertyDto.propertyId);

        return Ok(result);
    }
    
    [HttpPatch]
    [Route("api/papers/edit/{paperId}/properties/{propertyId}")]
    public async Task<ActionResult<bool>> AddPropertyToPaper([FromRoute] RemovePropertyDto removePropertyDto)
    {
        var result =
            await _paperService.AddPropertyToPaper(removePropertyDto.paperId, removePropertyDto.propertyId);

        return Ok(result);
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
    [Route("/api/admin/createProperty")]
    public async Task<ActionResult> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var propertyCreated = await _paperService.CreatePaperProperty(createPropertyDto.PropertyName);
        return Ok(propertyCreated);
    }
    
    //Todo check if the properties exists in the database
    [HttpPost]
    [Route("/api/admin/createPaper")]
    public async Task<ActionResult<PaperToDisplay>> CreatePaperProduct([FromBody] CreateProductDto createdProduct)
    {

        var receivedProduct = new PaperToAdd
        {
            Name = createdProduct.Name,
            Discontinued = createdProduct.Discontinued,
            Price = createdProduct.Price,
            Stock = createdProduct.Stock,
            PaperPropertiesList = createdProduct.PaperPropertiesList
        };

        var propertyCreated = await _paperService.CreatePaperProduct(receivedProduct);
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
        var editedProperty = await _paperService.EditPaperProperty(propertyToEdit);
        return Ok(editedProperty);
    }

    //delete paper property
    [HttpDelete]
    [Route("/api/admin/deletePaperPropriety")]
    public async Task<ActionResult> DeleteProperty([FromBody] EditPaperPropertyDto editPaperPropertyDto)
    {
        var deletedProperty =
            await _paperService.DeletePaperProperty(editPaperPropertyDto.PropertyId, editPaperPropertyDto.PropName!);
        var deleteResponse = new DeletePropertyResponse(deletedProperty, editPaperPropertyDto);
        return deleteResponse.ConstructDeleteResponse(ErrorMessages.GetMessage(ErrorCode.PropertyNotFound));
    }
}