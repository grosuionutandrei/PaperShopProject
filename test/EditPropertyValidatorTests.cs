using api.TransferModels;
using api.Validators;
using FluentValidation.TestHelper;
using utilities.ErrorMessages;

public class EditPropertyValidatorTests
{
    private readonly EditPropertyValidator _validator;

    public EditPropertyValidatorTests()
    {
        _validator = new EditPropertyValidator();
    }

    [Fact]
    public void EditPropertyValidator_ShouldPassValidation_WhenInputIsValid()
    {
        // Arrange
        var editPropertyDto = new EditPaperPropertyDto
        {
            PropertyId = 1,
            PropName = "Valid Property Name"
        };

        // Act
        var result = _validator.TestValidate(editPropertyDto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(property => property.PropertyId);
        result.ShouldNotHaveValidationErrorFor(property => property.PropName);
    }

    [Fact]
    public void EditPropertyValidator_ShouldFailValidation_WhenPropertyIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var editPropertyDto = new EditPaperPropertyDto
        {
            PropertyId = 0,
            PropName = "Valid Property Name"
        };

        // Act
        var result = _validator.TestValidate(editPropertyDto);

        // Assert
        result.ShouldHaveValidationErrorFor(property => property.PropertyId)
            .WithErrorMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EditPropertyValidator_ShouldFailValidation_WhenPropNameIsNullOrEmpty(string propName)
    {
        // Arrange
        var editPropertyDto = new EditPaperPropertyDto
        {
            PropertyId = 1,
            PropName = propName
        };

        // Act
        var result = _validator.TestValidate(editPropertyDto);

        // Assert
        result.ShouldHaveValidationErrorFor(property => property.PropName)
            .WithErrorMessage(ErrorMessages.GetMessage(ErrorCode.ErrorName));
    }
}