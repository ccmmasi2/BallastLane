using BallastLane.Test.API.Controllers;
using BallastLane.Test.Application.Common;
using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BallastLane.Test.Tests.Controllers
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _serviceMock;
        private readonly CustomerController    _sut;

        public CustomerControllerTests()
        {
            _serviceMock = new Mock<ICustomerService>();
            _sut         = new CustomerController(_serviceMock.Object);
        }

        private static CustomerDTO MakeDto(int id = 1, string fullName = "John Doe") => new()
        {
            Id             = id,
            FullName       = fullName,
            DocumentNumber = "12345678",
            Email          = "john@example.com",
            Phone          = "555-1234",
            Address        = "123 Main St"
        };

        // ── GetAll ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_WhenCustomersExist_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(new List<CustomerDTO> { MakeDto(1), MakeDto(2) });

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CustomerDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task GetAll_WhenNoCustomersExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CustomerDTO>());

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CustomerDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Data);
        }

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_WhenCustomerExists_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(MakeDto(1, "John Doe"));

            // Act
            var result   = await _sut.GetById(1);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CustomerDTO>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(1,          response.Data.Id);
            Assert.Equal("John Doe", response.Data.FullName);
        }

        [Fact]
        public async Task GetById_WhenCustomerDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((CustomerDTO?)null);

            // Act
            var result   = await _sut.GetById(99);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFound.Value);

            // Assert
            Assert.False(response.Success);
        }

        // ── Create ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_WhenDtoIsValid_ReturnsCreatedAtActionWithId()
        {
            // Arrange
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CustomerDTO>())).ReturnsAsync(7);

            // Act
            var result   = await _sut.Create(MakeDto(0));
            var created  = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<int>>(created.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(7,                  response.Data);
            Assert.Equal(nameof(_sut.GetById), created.ActionName);
        }

        // ── Update ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_SetsIdFromRouteParameter()
        {
            // Arrange
            CustomerDTO? captured = null;
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<CustomerDTO>()))
                        .Callback<CustomerDTO>(dto => captured = dto)
                        .Returns(Task.CompletedTask);

            // Act
            await _sut.Update(9, MakeDto(0, "Updated Name"));

            // Assert
            Assert.NotNull(captured);
            Assert.Equal(9, captured.Id);
        }

        [Fact]
        public async Task Update_WhenDtoIsValid_ReturnsOk()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<CustomerDTO>())).Returns(Task.CompletedTask);

            // Act
            var result   = await _sut.Update(1, MakeDto(1));
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(ok.Value);

            // Assert
            Assert.True(response.Success);
        }

        // ── Delete ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_WhenCustomerExists_ReturnsOk()
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
            await _sut.Delete(6);

            // Assert
            _serviceMock.Verify(s => s.DeleteAsync(6), Times.Once);
        }
    }
}
