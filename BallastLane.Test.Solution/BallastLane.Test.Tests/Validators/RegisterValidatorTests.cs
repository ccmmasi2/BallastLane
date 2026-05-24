using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Tests.Validators
{
    public class RegisterValidatorTests
    {
        private readonly RegisterValidator _sut = new();

        private static RegisterRequestDTO MakeDto(
            string username = "johndoe",
            string email    = "john@example.com",
            string password = "password123") => new()
        {
            Username = username,
            Email    = email,
            Password = password
        };

        [Fact]
        public void Validate_WhenAllFieldsAreValid_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() => _sut.Validate(MakeDto()));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_WhenUsernameIsNullOrWhiteSpace_ThrowsValidationException(string? username)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(username: username!)));

            // Assert
            Assert.Contains("username", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_WhenEmailIsNullOrWhiteSpace_ThrowsValidationException(string? email)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(email: email!)));

            // Assert
            Assert.Contains("email", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Validate_WhenEmailHasNoAtSign_ThrowsValidationException()
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(email: "invalidemail")));

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
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(password: password!)));

            // Assert
            Assert.Contains("password", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("12345")]
        public void Validate_WhenPasswordIsTooShort_ThrowsValidationException(string password)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(password: password)));

            // Assert
            Assert.Contains("6 characters", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Validate_WhenPasswordIsExactlyMinLength_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() => _sut.Validate(MakeDto(password: "123456")));

            Assert.Null(exception);
        }
    }
}
