using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Tests.Validators
{
    public class InvoiceValidatorTests
    {
        private readonly InvoiceValidator _sut = new();

        private static InvoiceDetailDTO MakeDetail(
            int productId = 1, int quantity = 2, decimal unitPrice = 10m) => new()
        {
            ProductId = productId,
            Quantity  = quantity,
            UnitPrice = unitPrice
        };

        private static InvoiceDTO MakeDto(int customerId = 1, int userId = 1) => new()
        {
            CustomerId      = customerId,
            CreatedByUserId = userId,
            Details         = new List<InvoiceDetailDTO> { MakeDetail() }
        };

        [Fact]
        public void Validate_WhenDtoIsValid_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() => _sut.Validate(MakeDto()));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_WhenCustomerIdIsNotPositive_ThrowsValidationException(int customerId)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(customerId: customerId)));

            // Assert
            Assert.Contains("customer", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_WhenCreatedByUserIdIsNotPositive_ThrowsValidationException(int userId)
        {
            // Arrange & Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(MakeDto(userId: userId)));

            // Assert
            Assert.Contains("user", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Validate_WhenDetailsIsEmpty_ThrowsValidationException()
        {
            // Arrange
            var dto = new InvoiceDTO { CustomerId = 1, CreatedByUserId = 1, Details = new List<InvoiceDetailDTO>() };

            // Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(dto));

            // Assert
            Assert.Contains("detail", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Validate_WhenDetailsIsNull_ThrowsValidationException()
        {
            // Arrange
            var dto = new InvoiceDTO { CustomerId = 1, CreatedByUserId = 1, Details = null! };

            // Act & Assert
            Assert.Throws<ValidationException>(() => _sut.Validate(dto));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_WhenDetailProductIdIsNotPositive_ThrowsValidationException(int productId)
        {
            // Arrange
            var dto = MakeDto();
            dto.Details = new List<InvoiceDetailDTO> { MakeDetail(productId: productId) };

            // Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(dto));

            // Assert
            Assert.Contains("product", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_WhenDetailQuantityIsNotPositive_ThrowsValidationException(int quantity)
        {
            // Arrange
            var dto = MakeDto();
            dto.Details = new List<InvoiceDetailDTO> { MakeDetail(quantity: quantity) };

            // Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(dto));

            // Assert
            Assert.Contains("quantity", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        public void Validate_WhenDetailUnitPriceIsNotPositive_ThrowsValidationException(decimal unitPrice)
        {
            // Arrange
            var dto = MakeDto();
            dto.Details = new List<InvoiceDetailDTO> { MakeDetail(unitPrice: unitPrice) };

            // Act
            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(dto));

            // Assert
            Assert.Contains("price", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Validate_WithMultipleValidDetails_DoesNotThrow()
        {
            // Arrange
            var dto = new InvoiceDTO
            {
                CustomerId = 1, CreatedByUserId = 1,
                Details    = new List<InvoiceDetailDTO>
                {
                    MakeDetail(1, 2, 50m),
                    MakeDetail(2, 1, 100m)
                }
            };

            // Act & Assert
            var exception = Record.Exception(() => _sut.Validate(dto));

            Assert.Null(exception);
        }
    }
}
