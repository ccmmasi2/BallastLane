using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Domain.Exceptions;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Moq;

namespace BallastLane.Test.Tests.Services
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository>  _invoiceRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly InvoiceService            _sut;

        public InvoiceServiceTests()
        {
            _invoiceRepoMock  = new Mock<IInvoiceRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _sut = new InvoiceService(
                _invoiceRepoMock.Object,
                _customerRepoMock.Object,
                new InvoiceValidator());
        }

        private static Customer MakeCustomer(int id = 1) => new()
        {
            Id             = id,
            FullName       = "John Doe",
            DocumentNumber = "12345678",
            Email          = "john@example.com",
            Phone          = "555-1234",
            Address        = "123 Main St"
        };

        private static InvoiceDTO MakeDto(int customerId = 1, int userId = 1) => new()
        {
            CustomerId      = customerId,
            CreatedByUserId = userId,
            Details         = new List<InvoiceDetailDTO>
            {
                new() { ProductId = 1, ProductName = "Laptop", CategoryName = "Electronics", UnitPrice = 999.99m, Quantity = 2 }
            }
        };

        private static Invoice MakeInvoice(int id = 1, int customerId = 1) => new()
        {
            Id               = id,
            CustomerId       = customerId,
            CreatedByUserId  = 1,
            InvoiceDate      = DateTime.UtcNow,
            Total            = 1999.98m,
            CustomerFullName = "John Doe",
            CustomerEmail    = "john@example.com",
            Details          = new List<InvoiceDetail>
            {
                new() { Id = 1, ProductId = 1, ProductName = "Laptop", CategoryName = "Electronics", UnitPrice = 999.99m, Quantity = 2, Subtotal = 1999.98m }
            }
        };

        // ── GetAllAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_WhenInvoicesExist_ReturnsMappedDtos()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetAllAsync())
                            .ReturnsAsync(new List<Invoice> { MakeInvoice(1), MakeInvoice(2) });

            // Act
            var result = (await _sut.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoInvoicesExist_ReturnsEmptyCollection()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Invoice>());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_MapsAllHeaderFieldsCorrectly()
        {
            // Arrange
            var invoice = MakeInvoice(7, 3);
            invoice.Total            = 500m;
            invoice.CustomerFullName = "Jane Roe";
            _invoiceRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Invoice> { invoice });

            // Act
            var result = (await _sut.GetAllAsync()).Single();

            // Assert
            Assert.Equal(7,          result.Id);
            Assert.Equal(3,          result.CustomerId);
            Assert.Equal(500m,       result.Total);
            Assert.Equal("Jane Roe", result.CustomerFullName);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenInvoiceExists_ReturnsMappedDto()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeInvoice(1));

            // Act
            var result = await _sut.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenInvoiceExists_MapsDetailsCorrectly()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeInvoice(1));

            // Act
            var result = await _sut.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Details);
            Assert.Equal("Laptop",      result.Details[0].ProductName);
            Assert.Equal("Electronics", result.Details[0].CategoryName);
            Assert.Equal(999.99m,       result.Details[0].UnitPrice);
            Assert.Equal(2,             result.Details[0].Quantity);
        }

        [Fact]
        public async Task GetByIdAsync_WhenInvoiceDoesNotExist_ReturnsNull()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Invoice?)null);

            // Act
            var result = await _sut.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        // ── GetByCustomerIdAsync ──────────────────────────────────────────────

        [Fact]
        public async Task GetByCustomerIdAsync_WhenInvoicesExist_ReturnsMappedDtos()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetByCustomerIdAsync(1))
                            .ReturnsAsync(new List<Invoice> { MakeInvoice(1, 1), MakeInvoice(2, 1) });

            // Act
            var result = (await _sut.GetByCustomerIdAsync(1)).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, i => Assert.Equal(1, i.CustomerId));
        }

        [Fact]
        public async Task GetByCustomerIdAsync_WhenNoInvoices_ReturnsEmptyCollection()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetByCustomerIdAsync(99)).ReturnsAsync(new List<Invoice>());

            // Act
            var result = await _sut.GetByCustomerIdAsync(99);

            // Assert
            Assert.Empty(result);
        }

        // ── CreateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_ReturnsNewId()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer());
            _invoiceRepoMock.Setup(r => r.CreateAsync(It.IsAny<Invoice>())).ReturnsAsync(42);

            // Act
            var newId = await _sut.CreateAsync(MakeDto());

            // Assert
            Assert.Equal(42, newId);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_SnapshotsCustomerData()
        {
            // Arrange
            var customer = MakeCustomer();
            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
            Invoice? captured = null;
            _invoiceRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => captured = i)
                .ReturnsAsync(1);

            // Act
            await _sut.CreateAsync(MakeDto());

            // Assert
            Assert.NotNull(captured);
            Assert.Equal(customer.FullName,       captured.CustomerFullName);
            Assert.Equal(customer.Email,          captured.CustomerEmail);
            Assert.Equal(customer.DocumentNumber, captured.CustomerDocumentNumber);
            Assert.Equal(customer.Phone,          captured.CustomerPhone);
            Assert.Equal(customer.Address,        captured.CustomerAddress);
        }

        [Fact]
        public async Task CreateAsync_RecalculatesSubtotalsServerSide()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer());
            Invoice? captured = null;
            _invoiceRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => captured = i)
                .ReturnsAsync(1);

            var dto = new InvoiceDTO
            {
                CustomerId = 1, CreatedByUserId = 1,
                Details = new List<InvoiceDetailDTO>
                {
                    new() { ProductId = 1, ProductName = "A", UnitPrice = 10m,  Quantity = 3, Subtotal = 0 },
                    new() { ProductId = 2, ProductName = "B", UnitPrice = 20m,  Quantity = 2, Subtotal = 0 }
                }
            };

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            Assert.NotNull(captured);
            var details = captured.Details.ToList();
            Assert.Equal(30m, details[0].Subtotal);
            Assert.Equal(40m, details[1].Subtotal);
            Assert.Equal(70m, captured.Total);
        }

        [Fact]
        public async Task CreateAsync_OverridesTamperedTotalFromClient()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer());
            Invoice? captured = null;
            _invoiceRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => captured = i)
                .ReturnsAsync(1);

            var dto = MakeDto();
            dto.Total = 999999m; // tampered

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            Assert.NotNull(captured);
            Assert.NotEqual(999999m, captured.Total);
        }

        [Fact]
        public async Task CreateAsync_SetsInvoiceDateToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;
            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer());
            Invoice? captured = null;
            _invoiceRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => captured = i)
                .ReturnsAsync(1);

            // Act
            await _sut.CreateAsync(MakeDto());

            // Assert
            Assert.NotNull(captured);
            Assert.True(captured.InvoiceDate >= before);
            Assert.True(captured.InvoiceDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task CreateAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(MakeDto(customerId: 99)));
            _invoiceRepoMock.Verify(r => r.CreateAsync(It.IsAny<Invoice>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_WhenCustomerIdIsNotPositive_ThrowsValidationException(int customerId)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(MakeDto(customerId: customerId)));
            _customerRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _invoiceRepoMock.Verify(r => r.CreateAsync(It.IsAny<Invoice>()),   Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_WhenCreatedByUserIdIsNotPositive_ThrowsValidationException(int userId)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(MakeDto(userId: userId)));
        }

        [Fact]
        public async Task CreateAsync_WhenDetailsAreEmpty_ThrowsValidationException()
        {
            // Arrange
            var dto = new InvoiceDTO { CustomerId = 1, CreatedByUserId = 1, Details = new List<InvoiceDetailDTO>() };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WhenDetailHasZeroQuantity_ThrowsValidationException()
        {
            // Arrange
            var dto = new InvoiceDTO
            {
                CustomerId = 1, CreatedByUserId = 1,
                Details    = new List<InvoiceDetailDTO>
                {
                    new() { ProductId = 1, UnitPrice = 10m, Quantity = 0 }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WhenDetailHasZeroUnitPrice_ThrowsValidationException()
        {
            // Arrange
            var dto = new InvoiceDTO
            {
                CustomerId = 1, CreatedByUserId = 1,
                Details    = new List<InvoiceDetailDTO>
                {
                    new() { ProductId = 1, UnitPrice = 0m, Quantity = 1 }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(dto));
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenInvoiceExists_CallsRepositoryDeleteOnce()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeInvoice(1));

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            _invoiceRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenInvoiceDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _invoiceRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Invoice?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(99));
            _invoiceRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
