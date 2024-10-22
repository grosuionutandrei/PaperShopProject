using api.TransferModels;
using api.Validators;
using Moq;
using service.Paper;
using utilities.ErrorMessages;

public class EditPaperValidatorTests
{
    private readonly Mock<IPaperService> _mockPaperService;
    private readonly EditPaperValidator _validator;

    public EditPaperValidatorTests()
    {
        _mockPaperService = new Mock<IPaperService>();
        _validator = new EditPaperValidator(_mockPaperService.Object);
    }

    [Fact]
    public void EditPaperValidator_ShouldPassValidation_WhenInputIsValid()
    {
        // Arrange
        _mockPaperService.Setup(x => x.ArePaperObjectsEqual(It.IsAny<int>())).ReturnsAsync(true);

        var editPaperDto = new EditPaperDto
        {
            Id = 1,
            Name = "Valid Paper",
            Price = 20.0,
            Stock = 10,
            Discontinued = true
        };

        // Act
        var result = _validator.Validate(editPaperDto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EditPaperValidator_ShouldFailValidation_WhenNameIsNullOrEmpty(string name)
    {
        // Arrange
        _mockPaperService.Setup(x => x.ArePaperObjectsEqual(It.IsAny<int>())).ReturnsAsync(true);

        var editPaperDto = new EditPaperDto
        {
            Id = 1,
            Name = name,
            Price = 20.0,
            Stock = 10,
            Discontinued = true
        };

        // Act
        var result = _validator.Validate(editPaperDto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.PropertyName == "Name" && e.ErrorMessage == ErrorMessages.GetMessage(ErrorCode.ErrorName));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void EditPaperValidator_ShouldFailValidation_WhenPriceIsLessThanOrEqualToZero(decimal price)
    {
        // Arrange
        _mockPaperService.Setup(x => x.ArePaperObjectsEqual(It.IsAny<int>())).ReturnsAsync(true);

        var editPaperDto = new EditPaperDto
        {
            Id = 1,
            Name = "Valid Paper",
            Price = 0,
            Stock = 10,
            Discontinued = true
        };

        // Act
        var result = _validator.Validate(editPaperDto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.PropertyName == "Price" && e.ErrorMessage == ErrorMessages.GetMessage(ErrorCode.Price));
    }

    [Fact]
    public void EditPaperValidator_ShouldFailValidation_WhenStockIsLessThanZero()
    {
        // Arrange
        _mockPaperService.Setup(x => x.ArePaperObjectsEqual(It.IsAny<int>())).ReturnsAsync(true);

        var editPaperDto = new EditPaperDto
        {
            Id = 1,
            Name = "Valid Paper",
            Price = 20.0,
            Stock = -1,
            Discontinued = true
        };

        // Act
        var result = _validator.Validate(editPaperDto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.PropertyName == "Stock" && e.ErrorMessage == ErrorMessages.GetMessage(ErrorCode.Stock));
    }

    [Fact]
    public void EditPaperValidator_ShouldFailValidation_WhenPaperIdDoesNotMatch()
    {
        // Arrange
        _mockPaperService.Setup(x => x.ArePaperObjectsEqual(It.IsAny<int>())).ReturnsAsync(false);

        var editPaperDto = new EditPaperDto
        {
            Id = 1,
            Name = "Valid Paper",
            Price = 20.0,
            Stock = 10,
            Discontinued = true
        };

        // Act
        var result = _validator.Validate(editPaperDto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.PropertyName == "Id" && e.ErrorMessage == ErrorMessages.GetMessage(ErrorCode.IdNotEqual));
    }
}