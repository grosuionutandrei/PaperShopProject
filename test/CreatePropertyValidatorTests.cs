using api.TransferModels;
using api.Validators;
using FluentValidation.TestHelper;
using Xunit;

public class CreatePropertyValidatorTests
{
    private readonly CreatePropertyValidator _validator;
    public CreatePropertyValidatorTests()
    {
        _validator = new CreatePropertyValidator();
    }

    [Fact]
    public void CreatePropertyValidator_ShouldPassValidation_WhenPropertyNameIsNotEmpty()
    {
        // Arrange
        var createPropertyDto = new CreatePropertyDto
        {
            PropertyName = "Valid Property Name"
        };

        // Act
        var result = _validator.TestValidate(createPropertyDto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(property => property.PropertyName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreatePropertyValidator_ShouldFailValidation_WhenPropertyNameIsNullOrEmpty(string propertyName)
    {
        // Arrange
        var createPropertyDto = new CreatePropertyDto
        {
            PropertyName = propertyName
        };

        // Act
        var result = _validator.TestValidate(createPropertyDto);

        // Assert
        result.ShouldHaveValidationErrorFor(property => property.PropertyName)
            .WithErrorMessage("Property name is required");
    }
}