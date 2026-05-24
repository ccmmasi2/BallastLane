using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Domain.Exceptions;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Moq;

namespace BallastLane.Test.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _repositoryMock;
        private readonly CategoryService           _sut;

        public CategoryServiceTests()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _sut = new CategoryService(_repositoryMock.Object, new CategoryValidator());
        }

        private static Category MakeCategory(int id = 1, string name = "Electronics") => new()
        {
            Id          = id,
            Name        = name,
            Description = "Sample description"
        };

        private static CategoryDTO MakeDto(int id = 0, string name = "Electronics") => new()
        {
            Id          = id,
            Name        = name,
            Description = "Sample description"
        };

        // ── GetAllAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_WhenCategoriesExist_ReturnsMappedDtos()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Category> { MakeCategory(1, "Electronics"), MakeCategory(2, "Clothing") });

            // Act
            var result = (await _sut.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Electronics");
            Assert.Contains(result, c => c.Name == "Clothing");
        }

        [Fact]
        public async Task GetAllAsync_WhenNoCategoriesExist_ReturnsEmptyCollection()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_MapsAllFieldsCorrectly()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Category> { new() { Id = 5, Name = "Books", Description = "All books" } });

            // Act
            var result = (await _sut.GetAllAsync()).Single();

            // Assert
            Assert.Equal(5,          result.Id);
            Assert.Equal("Books",    result.Name);
            Assert.Equal("All books", result.Description);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenCategoryExists_ReturnsMappedDto()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCategory(1, "Electronics"));

            // Act
            var result = await _sut.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1,             result.Id);
            Assert.Equal("Electronics", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

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
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Category>())).ReturnsAsync(10);

            // Act
            var newId = await _sut.CreateAsync(MakeDto());

            // Assert
            Assert.Equal(10, newId);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_CallsRepositoryOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Category>())).ReturnsAsync(1);

            // Act
            await _sut.CreateAsync(MakeDto());

            // Assert
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_MapsAllFieldsToEntity()
        {
            // Arrange
            var dto = new CategoryDTO { Name = "Tools", Description = "Hand tools" };
            Category? captured = null;
            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .Callback<Category>(c => captured = c)
                .ReturnsAsync(1);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            Assert.NotNull(captured);
            Assert.Equal("Tools",      captured.Name);
            Assert.Equal("Hand tools", captured.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WhenNameIsNullOrWhiteSpace_ThrowsValidationException(string? name)
        {
            // Arrange
            var dto = MakeDto(name: name!);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(dto));
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Never);
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_WhenCategoryExistsAndDtoIsValid_CallsRepositoryUpdateOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCategory(1));

            // Act
            await _sut.UpdateAsync(MakeDto(id: 1, name: "Updated"));

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryExistsAndDtoIsValid_MapsAllFieldsToEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCategory(1));
            Category? captured = null;
            _repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .Callback<Category>(c => captured = c);

            // Act
            await _sut.UpdateAsync(new CategoryDTO { Id = 1, Name = "Updated", Description = "New desc" });

            // Assert
            Assert.NotNull(captured);
            Assert.Equal(1,         captured.Id);
            Assert.Equal("Updated", captured.Name);
            Assert.Equal("New desc", captured.Description);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(MakeDto(id: 99)));
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenValidationFails_ThrowsValidationException_BeforeHittingRepository()
        {
            // Arrange
            var dto = new CategoryDTO { Id = 1, Name = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(dto));
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()),  Times.Never);
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenCategoryExists_CallsRepositoryDeleteOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCategory(1));

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenCategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(99));
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCategoryExists_DoesNotDeleteOtherCategories()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCategory(1));

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(It.Is<int>(id => id != 1)), Times.Never);
        }
    }
}
