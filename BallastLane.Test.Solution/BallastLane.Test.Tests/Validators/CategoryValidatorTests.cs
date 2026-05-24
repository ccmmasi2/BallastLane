using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Tests.Validators
{
    public class CategoryValidatorTests
    {
        private readonly CategoryValidator _sut = new();

        [Fact]
        public void Validate_WhenNameIsValid_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() => _sut.Validate(new CategoryDTO { Name = "Electronics" }));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_WhenNameIsNullOrWhiteSpace_ThrowsValidationException(string? name)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(new CategoryDTO { Name = name! }));

            // Assert
            Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
