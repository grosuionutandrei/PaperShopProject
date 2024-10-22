using api.Controllers;
using api.TransferModels;
using infrastructure.QuerryModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using service.Paper;

public class PaperControllerTests
{
    private readonly PaperController _controller;
    private readonly Mock<ILogger<PaperController>> _mockLogger;
    private readonly Mock<IPaperService> _mockPaperService;

    public PaperControllerTests()
    {
        _mockPaperService = new Mock<IPaperService>();
        _mockLogger = new Mock<ILogger<PaperController>>();
        _controller = new PaperController(_mockPaperService.Object, _mockLogger.Object);
    }

    [Fact]
    public void GetPaper_ShouldReturnOk_WhenPapersExist()
    {
        // Arrange
        var pageNumber = 1;
        var paginationQuery = new PaperPaginationQueryDto { PageItems = 10 };
        _mockPaperService.Setup(service => service.GetPaperWithQuerries(pageNumber, paginationQuery.PageItems))
            .Returns(new List<PaperToDisplay> { new() { Id = 1, Name = "Paper A" } });

        // Act
        var result = _controller.GetPaper(pageNumber, paginationQuery);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPapers = Assert.IsAssignableFrom<IEnumerable<PaperToDisplay>>(okResult.Value);
        Assert.Single(returnedPapers);
    }

    [Fact]
    public async Task GetPaperByFilter_ShouldReturnBadRequest_WhenFilterDtoIsNull()
    {
        // Arrange
        PaperFilterDto filterDto = null;

        // Act
        var result = await _controller.GetPaperByFilter(filterDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("PaperFilterDto cannot be null.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetPaperByFilter_ShouldReturnBadRequest_WhenPaginationIsNull()
    {
        // Arrange
        var filterDto = new PaperFilterDto
        {
            pagination = null,
            priceRange = new PriceRangeDto { minimumRange = 5, maximumRange = 100 }
        };

        // Act
        var result = await _controller.GetPaperByFilter(filterDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Pagination cannot be null.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetPriceRange_ShouldReturnOk_WhenPriceRangeExists()
    {
        // Arrange
        var priceRange = new PriceRange { minimumRange = 5, maximumRange = 100 };
        _mockPaperService.Setup(service => service.GetPriceRange()).ReturnsAsync(priceRange);

        // Act
        var result = await _controller.GetPriceRange();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPriceRange = Assert.IsType<PriceRange>(okResult.Value);
        Assert.Equal(priceRange.minimumRange, returnedPriceRange.minimumRange);
        Assert.Equal(priceRange.maximumRange, returnedPriceRange.maximumRange);
    }

    [Fact]
    public async Task EditPaper_ShouldReturnOk_WhenPaperIsEditedSuccessfully()
    {
        // Arrange
        var editPaperDto = new EditPaperDto
        {
            Id = 1,
            Name = "Edited Paper",
            Price = 20.0,
            Discontinued = false,
            Stock = 10
        };
        _mockPaperService.Setup(service => service.EditPaper(It.IsAny<PaperToDisplay>())).ReturnsAsync(true);

        // Act
        var result = await _controller.EditPaper(editPaperDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPaper = Assert.IsType<EditPaperDto>(okResult.Value);
        Assert.Equal(editPaperDto.Id, returnedPaper.Id);
    }

    [Fact]
    public async Task DeletePaper_ShouldReturnOk_WhenPaperIsDeleted()
    {
        // Arrange
        var paperId = 1;
        _mockPaperService.Setup(service => service.DeletePaperById(paperId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeletePaper(paperId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task DeletePaper_ShouldReturnNotFound_WhenPaperIsNotDeleted()
    {
        // Arrange
        var paperId = 1;
        _mockPaperService.Setup(service => service.DeletePaperById(paperId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeletePaper(paperId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.False((bool)notFoundResult.Value);
    }

    [Fact]
    public async Task GetPaperByFilter_ShouldReturnOk_WhenFilterIsValid()
    {
        // Arrange
        var filterDto = new PaperFilterDto
        {
            pagination = new PaperPaginationQueryDto { PageNumber = 1, PageItems = 10 },
            priceRange = new PriceRangeDto { minimumRange = 5, maximumRange = 100 },
            paperPropertiesIds = "1,2,3",
            searchFilter = "Paper"
        };

        var expectedPapers = new List<PaperToDisplay>
        {
            new() { Id = 1, Name = "Filtered Paper" }
        };

        _mockPaperService.Setup(service => service.GetPapersByFilter(It.IsAny<PaperFilterQuery>()))
            .ReturnsAsync(expectedPapers);

        // Act
        var result = await _controller.GetPaperByFilter(filterDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPapers = Assert.IsAssignableFrom<IEnumerable<PaperToDisplay>>(okResult.Value);
        Assert.Single(returnedPapers);
        Assert.Equal(expectedPapers[0].Id, returnedPapers.First().Id);
        Assert.Equal(expectedPapers[0].Name, returnedPapers.First().Name);
    }
}