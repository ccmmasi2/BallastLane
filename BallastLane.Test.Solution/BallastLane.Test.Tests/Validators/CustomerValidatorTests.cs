using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Tests.Validators
{
    public class CustomerValidatorTests
    {
        private readonly CustomerValidator _sut = new();

        [Fact]
        public void Validate_WhenAllRequiredFieldsAreValid_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() =>
                _sut.Validate(new CustomerDTO { FullName = "John Doe", Email = "john@example.com" }));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenEmailIsNull_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() =>
                _sut.Validate(new CustomerDTO { FullName = "John Doe", Email = null }));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_WhenFullNameIsNullOrWhiteSpace_ThrowsValidationException(string? fullName)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() =>
                _sut.Validate(new CustomerDTO { FullName = fullName! }));

            // Assert
            Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("missingatsign.com")]
        public void Validate_WhenEmailIsInvalid_ThrowsValidationException(string email)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() =>
                _sut.Validate(new CustomerDTO { FullName = "John Doe", Email = email }));

            // Assert
            Assert.Contains("email", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
