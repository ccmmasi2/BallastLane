using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Moq;

namespace BallastLane.Test.Tests.Services
{
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly ProductService _sut;

        public ProductServiceTest()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _sut = new ProductService(_repositoryMock.Object, new ProductValidator());
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static Product MakeProduct(
            int id = 1, string name = "Laptop",
            decimal price = 999.99m, int categoryId = 1) => new()
        {
            Id          = id,
            CategoryId  = categoryId,
            Name        = name,
            Description = "Sample description",
            Price       = price,
            //Category    = new Category { Id = categoryId, Name = "Electronics" }
        };

        private static ProductDTO MakeDto(
            int id = 0, string name = "Laptop",
            decimal price = 999.99m, int categoryId = 1) => new()
        {
            Id           = id,
            CategoryId   = categoryId,
            CategoryName = "Electronics",
            Name         = name,
            Description  = "Sample description",
            Price        = price
        };

        // ── GetAllAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_WhenProductsExist_ReturnsMappedDtos()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Product> { MakeProduct(1, "Laptop"), MakeProduct(2, "Mouse") });

            // Act
            var result = (await _sut.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name == "Laptop");
            Assert.Contains(result, p => p.Name == "Mouse");
        }

        [Fact]
        public async Task GetAllAsync_WhenNoProductsExist_ReturnsEmptyCollection()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Product>());

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
                .ReturnsAsync(new List<Product> { MakeProduct(7, "Keyboard", 149.99m, 3) });

            // Act
            var result = (await _sut.GetAllAsync()).Single();

            // Assert
            Assert.Equal(7, result.Id);
            Assert.Equal("Keyboard", result.Name);
            Assert.Equal(149.99m, result.Price);
            Assert.Equal(3, result.CategoryId);
            Assert.Equal("Electronics", result.CategoryName);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenProductExists_ReturnsMappedDto()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(MakeProduct(1, "Laptop", 999.99m, 2));

            // Act
            var result = await _sut.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Laptop", result.Name);
            Assert.Equal(999.99m, result.Price);
            Assert.Equal(2, result.CategoryId);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _sut.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        // ── GetByCategoryIdAsync ──────────────────────────────────────────────

        [Fact]
        public async Task GetByCategoryIdAsync_WhenProductsExist_ReturnsMappedDtos()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByCategoryIdAsync(5))
                .ReturnsAsync(new List<Product> { MakeProduct(1, categoryId: 5), MakeProduct(2, categoryId: 5) });

            // Act
            var result = (await _sut.GetByCategoryIdAsync(5)).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(5, p.CategoryId));
        }

        [Fact]
        public async Task GetByCategoryIdAsync_WhenNoCategoryProducts_ReturnsEmptyCollection()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByCategoryIdAsync(99))
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _sut.GetByCategoryIdAsync(99);

            // Assert
            Assert.Empty(result);
        }

        // ── CreateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_ReturnsNewId()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Product>()))
                .ReturnsAsync(42);

            // Act
            var newId = await _sut.CreateAsync(MakeDto());

            // Assert
            Assert.Equal(42, newId);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_CallsRepositoryOnce()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Product>()))
                .ReturnsAsync(1);

            // Act
            await _sut.CreateAsync(MakeDto());

            // Assert
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsValid_MapsAllFieldsToEntity()
        {
            // Arrange
            var dto = new ProductDTO
            {
                Name        = "Gaming Chair",
                CategoryId  = 3,
                Price       = 399.99m,
                Description = "Ergonomic"
            };
            Product? captured = null;
            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Product>()))
                .Callback<Product>(p => captured = p)
                .ReturnsAsync(1);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            Assert.NotNull(captured);
            Assert.Equal("Gaming Chair", captured.Name);
            Assert.Equal(3, captured.CategoryId);
            Assert.Equal(399.99m, captured.Price);
            Assert.Equal("Ergonomic", captured.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WhenNameIsNullOrWhiteSpace_ThrowsArgumentException(string? name)
        {
            // Arrange
            var dto = MakeDto(name: name!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(dto));
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-500)]
        public async Task CreateAsync_WhenPriceIsNotPositive_ThrowsArgumentException(decimal price)
        {
            // Arrange
            var dto = MakeDto(price: price);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(dto));
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_WhenCategoryIdIsNotPositive_ThrowsArgumentException(int categoryId)
        {
            // Arrange
            var dto = MakeDto(categoryId: categoryId);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(dto));
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_WhenProductExistsAndDtoIsValid_CallsRepositoryUpdateOnce()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(MakeProduct(1));

            // Act
            await _sut.UpdateAsync(MakeDto(id: 1, name: "Updated Laptop", price: 1099m));

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductExistsAndDtoIsValid_MapsAllFieldsToEntity()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(MakeProduct(1));

            Product? captured = null;
            _repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                .Callback<Product>(p => captured = p);

            var dto = new ProductDTO
            {
                Id          = 1,
                Name        = "Updated Laptop",
                CategoryId  = 2,
                Price       = 1099m,
                Description = "Updated description"
            };

            // Act
            await _sut.UpdateAsync(dto);

            // Assert
            Assert.NotNull(captured);
            Assert.Equal(1, captured.Id);
            Assert.Equal("Updated Laptop", captured.Name);
            Assert.Equal(2, captured.CategoryId);
            Assert.Equal(1099m, captured.Price);
            Assert.Equal("Updated description", captured.Description);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateAsync(MakeDto(id: 99)));
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenValidationFails_ThrowsArgumentException_BeforeHittingRepository()
        {
            // Arrange
            var dto = new ProductDTO { Id = 1, Name = "", CategoryId = 1, Price = 100m };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateAsync(dto));
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenProductExists_CallsRepositoryDeleteOnce()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(MakeProduct(1));

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(99));
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductExists_DoesNotDeleteOtherProducts()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(MakeProduct(1));

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(It.Is<int>(id => id != 1)), Times.Never);
        }
    }
}
