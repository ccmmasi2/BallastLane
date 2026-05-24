using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Domain.Exceptions;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Moq;

namespace BallastLane.Test.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly CustomerService           _sut;

        public CustomerServiceTests()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _sut = new CustomerService(_repositoryMock.Object, new CustomerValidator());
        }

        private static Customer MakeCustomer(int id = 1, string fullName = "John Doe") => new()
        {
            Id             = id,
            FullName       = fullName,
            DocumentNumber = "12345678",
            Email          = "john@example.com",
            Phone          = "555-1234",
            Address        = "123 Main St"
        };

        private static CustomerDTO MakeDto(
            int id = 0, string fullName = "John Doe",
            string? email = "john@example.com") => new()
        {
            Id             = id,
            FullName       = fullName,
            DocumentNumber = "12345678",
            Email          = email,
            Phone          = "555-1234",
            Address        = "123 Main St"
        };

        // ── GetAllAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_WhenCustomersExist_ReturnsMappedDtos()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Customer> { MakeCustomer(1, "John Doe"), MakeCustomer(2, "Jane Doe") });

            // Act
            var result = (await _sut.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.FullName == "John Doe");
            Assert.Contains(result, c => c.FullName == "Jane Doe");
        }

        [Fact]
        public async Task GetAllAsync_WhenNoCustomersExist_ReturnsEmptyCollection()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Customer>());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_MapsAllFieldsCorrectly()
        {
            // Arrange
            var customer = new Customer
            {
                Id             = 3,
                FullName       = "Alice Smith",
                DocumentNumber = "99999999",
                Email          = "alice@example.com",
                Phone          = "555-9999",
                Address        = "456 Oak Ave"
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Customer> { customer });

            // Act
            var result = (await _sut.GetAllAsync()).Single();

            // Assert
            Assert.Equal(3,                   result.Id);
            Assert.Equal("Alice Smith",        result.FullName);
            Assert.Equal("99999999",           result.DocumentNumber);
            Assert.Equal("alice@example.com",  result.Email);
            Assert.Equal("555-9999",           result.Phone);
            Assert.Equal("456 Oak Ave",        result.Address);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenCustomerExists_ReturnsMappedDto()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer(1));

            // Act
            var result = await _sut.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1,          result.Id);
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            // Act
            var result = await _sut.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        // ── CreateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_ReturnsNewId()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Customer>())).ReturnsAsync(5);

            // Act
            var newId = await _sut.CreateAsync(MakeDto());

            // Assert
            Assert.Equal(5, newId);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_CallsRepositoryOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Customer>())).ReturnsAsync(1);

            // Act
            await _sut.CreateAsync(MakeDto());

            // Assert
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WhenFullNameIsNullOrWhiteSpace_ThrowsValidationException(string? fullName)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(MakeDto(fullName: fullName!)));
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("missingatsign.com")]
        public async Task CreateAsync_WhenEmailIsInvalid_ThrowsValidationException(string email)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(MakeDto(email: email)));
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailIsNull_DoesNotThrow()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Customer>())).ReturnsAsync(1);

            // Act
            var newId = await _sut.CreateAsync(MakeDto(email: null));

            // Assert
            Assert.Equal(1, newId);
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_WhenCustomerExistsAndDtoIsValid_CallsRepositoryUpdateOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer(1));

            // Act
            await _sut.UpdateAsync(MakeDto(id: 1, fullName: "Updated Name"));

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(MakeDto(id: 99)));
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenValidationFails_ThrowsValidationException_BeforeHittingRepository()
        {
            // Arrange
            var dto = new CustomerDTO { Id = 1, FullName = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(dto));
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()),  Times.Never);
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenCustomerExists_CallsRepositoryDeleteOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer(1));

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(99));
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
