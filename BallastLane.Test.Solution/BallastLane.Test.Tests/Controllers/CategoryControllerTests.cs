using BallastLane.Test.API.Controllers;
using BallastLane.Test.Application.Common;
using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BallastLane.Test.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _serviceMock;
        private readonly CategoryController    _sut;

        public CategoryControllerTests()
        {
            _serviceMock = new Mock<ICategoryService>();
            _sut         = new CategoryController(_serviceMock.Object);
        }

        private static CategoryDTO MakeDto(int id = 1, string name = "Electronics") => new()
        {
            Id          = id,
            Name        = name,
            Description = "Sample description"
        };

        // ── GetAll ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_WhenCategoriesExist_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(new List<CategoryDTO> { MakeDto(1), MakeDto(2) });

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CategoryDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task GetAll_WhenNoCategoriesExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CategoryDTO>());

            // Act
            var result   = await _sut.GetAll();
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CategoryDTO>>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Data);
        }

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_WhenCategoryExists_ReturnsOkWithData()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(MakeDto(1, "Electronics"));

            // Act
            var result   = await _sut.GetById(1);
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CategoryDTO>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(1,             response.Data.Id);
            Assert.Equal("Electronics", response.Data.Name);
        }

        [Fact]
        public async Task GetById_WhenCategoryDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((CategoryDTO?)null);

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
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CategoryDTO>())).ReturnsAsync(5);

            // Act
            var result   = await _sut.Create(MakeDto(0));
            var created  = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<int>>(created.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(5,                  response.Data);
            Assert.Equal(nameof(_sut.GetById), created.ActionName);
        }

        [Fact]
        public async Task Create_WhenDtoIsValid_CallsServiceOnce()
        {
            // Arrange
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CategoryDTO>())).ReturnsAsync(1);

            // Act
            await _sut.Create(MakeDto(0));

            // Assert
            _serviceMock.Verify(s => s.CreateAsync(It.IsAny<CategoryDTO>()), Times.Once);
        }

        // ── Update ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_WhenDtoIsValid_ReturnsOk()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<CategoryDTO>())).Returns(Task.CompletedTask);

            // Act
            var result   = await _sut.Update(1, MakeDto(0, "Updated"));
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(ok.Value);

            // Assert
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Update_SetsIdFromRouteParameter()
        {
            // Arrange
            CategoryDTO? captured = null;
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<CategoryDTO>()))
                        .Callback<CategoryDTO>(dto => captured = dto)
                        .Returns(Task.CompletedTask);

            // Act
            await _sut.Update(7, MakeDto(0, "Updated"));

            // Assert — route id overwrites whatever was in the DTO
            Assert.NotNull(captured);
            Assert.Equal(7, captured.Id);
        }

        [Fact]
        public async Task Update_WhenDtoIsValid_CallsServiceOnce()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<CategoryDTO>())).Returns(Task.CompletedTask);

            // Act
            await _sut.Update(1, MakeDto(1));

            // Assert
            _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<CategoryDTO>()), Times.Once);
        }

        // ── Delete ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_WhenCategoryExists_ReturnsOk()
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
        public async Task Delete_WhenCategoryExists_CallsServiceWithCorrectId()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            await _sut.Delete(3);

            // Assert
            _serviceMock.Verify(s => s.DeleteAsync(3), Times.Once);
        }
    }
}
