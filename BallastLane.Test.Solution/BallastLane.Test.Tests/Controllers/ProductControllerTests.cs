using BallastLane.Test.API.Controllers;
using BallastLane.Test.Application.Common;
using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BallastLane.Test.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _serviceMock;
        private readonly ProductController    _sut;

        public ProductControllerTests()
        {
            _serviceMock = new Mock<IProductService>();
            _sut         = new ProductController(_serviceMock.Object);
        }

        private static ProductDTO MakeDto(int id = 1, string name = "Laptop", int categoryId = 1) => new()
        {
            Id           = id,
            Name         = name,
            CategoryId   = categoryId,
            CategoryName = "Electronics",
            Description  = "Sample description",
            Price        = 999.99m
        };

        // ── GetAll ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_WhenProductsExist_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(new List<ProductDTO> { MakeDto(1), MakeDto(2) });

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task GetAll_WhenNoProductsExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ProductDTO>());

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Data);
        }

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_WhenProductExists_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(MakeDto(1, "Laptop"));

            // Act
            var result   = await _sut.GetById(1);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(1,        response.Data.Id);
            Assert.Equal("Laptop", response.Data.Name);
        }

        [Fact]
        public async Task GetById_WhenProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ProductDTO?)null);

            // Act
            var result   = await _sut.GetById(99);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFound.Value);

            // Assert
            Assert.False(response.Success);
        }

        // ── GetByCategory ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetByCategory_WhenProductsExist_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByCategoryIdAsync(2))
                        .ReturnsAsync(new List<ProductDTO> { MakeDto(1, categoryId: 2), MakeDto(2, categoryId: 2) });

            // Act
            var result   = await _sut.GetByCategory(2);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task GetByCategory_WhenNoProducts_ReturnsOkWithEmptyList()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByCategoryIdAsync(99)).ReturnsAsync(new List<ProductDTO>());

            // Act
            var result   = await _sut.GetByCategory(99);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Data);
        }

        // ── Create ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_WhenDtoIsValid_ReturnsCreatedAtActionWithId()
        {
            // Arrange
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<ProductDTO>())).ReturnsAsync(10);

            // Act
            var result   = await _sut.Create(MakeDto(0));
            var created  = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<int>>(created.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(10,                 response.Data);
            Assert.Equal(nameof(_sut.GetById), created.ActionName);
        }

        // ── Update ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_SetsIdFromRouteParameter()
        {
            // Arrange
            ProductDTO? captured = null;
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<ProductDTO>()))
                        .Callback<ProductDTO>(dto => captured = dto)
                        .Returns(Task.CompletedTask);

            // Act
            await _sut.Update(5, MakeDto(0, "Updated Laptop"));

            // Assert
            Assert.NotNull(captured);
            Assert.Equal(5, captured.Id);
        }

        [Fact]
        public async Task Update_WhenDtoIsValid_ReturnsOk()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<ProductDTO>())).Returns(Task.CompletedTask);

            // Act
            var result   = await _sut.Update(1, MakeDto(1));
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(ok.Value);

            // Assert
            Assert.True(response.Success);
        }

        // ── Delete ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_WhenProductExists_ReturnsOk()
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
            await _sut.Delete(4);

            // Assert
            _serviceMock.Verify(s => s.DeleteAsync(4), Times.Once);
        }
    }
}
