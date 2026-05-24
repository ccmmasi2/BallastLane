using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Tests.Validators
{
    public class LoginValidatorTests
    {
        private readonly LoginValidator _sut = new();

        [Fact]
        public void Validate_WhenAllFieldsAreValid_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() =>
                _sut.Validate(new LoginRequestDTO { Email = "john@example.com", Password = "anypass" }));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_WhenEmailIsNullOrWhiteSpace_ThrowsValidationException(string? email)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() =>
                _sut.Validate(new LoginRequestDTO { Email = email!, Password = "anypass" }));

            // Assert
            Assert.Contains("email", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_WhenPasswordIsNullOrWhiteSpace_ThrowsValidationException(string? password)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() =>
                _sut.Validate(new LoginRequestDTO { Email = "john@example.com", Password = password! }));

            // Assert
            Assert.Contains("password", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
