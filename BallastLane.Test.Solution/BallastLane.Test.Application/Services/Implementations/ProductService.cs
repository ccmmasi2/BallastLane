using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;

namespace BallastLane.Test.Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ProductValidator _validator;

        public ProductService(IProductRepository repository, ProductValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDTO?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product is null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetByCategoryIdAsync(int categoryId)
        {
            var products = await _repository.GetByCategoryIdAsync(categoryId);
            return products.Select(MapToDto);
        }

        public async Task<int> CreateAsync(ProductDTO dto)
        {
            _validator.Validate(dto);
            return await _repository.CreateAsync(MapToEntity(dto));
        }

        public async Task UpdateAsync(ProductDTO dto)
        {
            _validator.Validate(dto);

            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing is null)
                throw new KeyNotFoundException($"Product with id {dto.Id} was not found.");

            await _repository.UpdateAsync(MapToEntity(dto));
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                throw new KeyNotFoundException($"Product with id {id} was not found.");

            await _repository.DeleteAsync(id);
        }

        private static ProductDTO MapToDto(Product p) => new()
        {
            Id           = p.Id,
            CategoryId   = p.CategoryId,
            //CategoryName = p.Category?.Name ?? string.Empty,
            Name         = p.Name,
            Description  = p.Description,
            Price        = p.Price
        };

        private static Product MapToEntity(ProductDTO dto) => new()
        {
            Id          = dto.Id,
            CategoryId  = dto.CategoryId,
            Name        = dto.Name,
            Description = dto.Description,
            Price       = dto.Price
        };
    }
}
