using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Tests.Validators
{
    public class ProductValidatorTests
    {
        private readonly ProductValidator _sut = new();

        private static ProductDTO MakeDto(
            string name = "Laptop", int categoryId = 1, decimal price = 99.99m) => new()
        {
            Name       = name,
            CategoryId = categoryId,
            Price      = price
        };

        [Fact]
        public void Validate_WhenDtoIsValid_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() => _sut.Validate(MakeDto()));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_WhenNameIsNullOrWhiteSpace_ThrowsValidationException(string? name)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(name: name!)));

            // Assert
            Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Validate_WhenCategoryIdIsNotPositive_ThrowsValidationException(int categoryId)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(categoryId: categoryId)));

            // Assert
            Assert.Contains("category", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-500)]
        public void Validate_WhenPriceIsNotPositive_ThrowsValidationException(decimal price)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(price: price)));

            // Assert
            Assert.Contains("price", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Validate_WhenPriceIsMinimumPositive_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() => _sut.Validate(MakeDto(price: 0.01m)));

            Assert.Null(exception);
        }
    }
}
