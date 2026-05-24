using BallastLane.Test.API.Controllers;
using BallastLane.Test.Application.Common;
using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BallastLane.Test.Tests.Controllers
{
    public class InvoiceControllerTests
    {
        private readonly Mock<IInvoiceService> _serviceMock;
        private readonly InvoiceController    _sut;

        public InvoiceControllerTests()
        {
            _serviceMock = new Mock<IInvoiceService>();
            _sut         = new InvoiceController(_serviceMock.Object);
        }

        private void SetAuthenticatedUser(int userId)
        {
            var claims    = new[] { new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()) };
            var identity  = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);

            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        private static InvoiceDTO MakeDto(int id = 1, int customerId = 1) => new()
        {
            Id              = id,
            CustomerId      = customerId,
            CreatedByUserId = 1,
            InvoiceDate     = DateTime.UtcNow,
            Total           = 1999.98m,
            CustomerFullName = "John Doe",
            Details          = new List<InvoiceDetailDTO>
            {
                new() { ProductId = 1, ProductName = "Laptop", UnitPrice = 999.99m, Quantity = 2, Subtotal = 1999.98m }
            }
        };

        // ── GetAll ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_WhenInvoicesExist_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(new List<InvoiceDTO> { MakeDto(1), MakeDto(2) });

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<InvoiceDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task GetAll_WhenNoInvoicesExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<InvoiceDTO>());

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<InvoiceDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Data);
        }

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_WhenInvoiceExists_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(MakeDto(1));

            // Act
            var result   = await _sut.GetById(1);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<InvoiceDTO>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task GetById_WhenInvoiceDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((InvoiceDTO?)null);

            // Act
            var result   = await _sut.GetById(99);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFound.Value);

            // Assert
            Assert.False(response.Success);
        }

        // ── GetByCustomer ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetByCustomer_WhenInvoicesExist_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByCustomerIdAsync(3))
                        .ReturnsAsync(new List<InvoiceDTO> { MakeDto(1, 3), MakeDto(2, 3) });

            // Act
            var result   = await _sut.GetByCustomer(3);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<InvoiceDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(2, response.Data.Count());
        }

        // ── Create ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_WhenDtoIsValid_ReturnsCreatedAtActionWithId()
        {
            // Arrange
            SetAuthenticatedUser(42);
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<InvoiceDTO>())).ReturnsAsync(99);

            // Act
            var result   = await _sut.Create(MakeDto(0));
            var created  = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<int>>(created.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(99,                 response.Data);
            Assert.Equal(nameof(_sut.GetById), created.ActionName);
        }

        [Fact]
        public async Task Create_SetsCreatedByUserIdFromJwtClaim()
        {
            // Arrange
            SetAuthenticatedUser(42);
            InvoiceDTO? captured = null;
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<InvoiceDTO>()))
                        .Callback<InvoiceDTO>(dto => captured = dto)
                        .ReturnsAsync(1);

            // Act
            var dto = MakeDto(0);
            dto.CreatedByUserId = 0; // simulate client sending wrong value
            await _sut.Create(dto);

            // Assert — controller overwrites with JWT claim value
            Assert.NotNull(captured);
            Assert.Equal(42, captured.CreatedByUserId);
        }

        [Fact]
        public async Task Create_WhenNoJwtClaim_SetsCreatedByUserIdToZero()
        {
            // Arrange — no SetAuthenticatedUser call
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };
            InvoiceDTO? captured = null;
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<InvoiceDTO>()))
                        .Callback<InvoiceDTO>(dto => captured = dto)
                        .ReturnsAsync(1);

            // Act
            await _sut.Create(MakeDto(0));

            // Assert
            Assert.NotNull(captured);
            Assert.Equal(0, captured.CreatedByUserId);
        }

        // ── Delete ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_WhenInvoiceExists_ReturnsOk()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result   = await _sut.Delete(1);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(ok.Value);

            // Assert
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Delete_CallsServiceWithCorrectId()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            await _sut.Delete(5);

            // Assert
            _serviceMock.Verify(s => s.DeleteAsync(5), Times.Once);
        }
    }
}
