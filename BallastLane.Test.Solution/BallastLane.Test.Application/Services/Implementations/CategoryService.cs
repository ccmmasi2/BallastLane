using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;

namespace BallastLane.Test.Application.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly CategoryValidator   _validator;

        public CategoryService(ICategoryRepository repository, CategoryValidator validator)
        {
            _repository = repository;
            _validator  = validator;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category is null ? null : MapToDto(category);
        }

        public async Task<int> CreateAsync(CategoryDTO dto)
        {
            _validator.Validate(dto);
            return await _repository.CreateAsync(MapToEntity(dto));
        }

        public async Task UpdateAsync(CategoryDTO dto)
        {
            _validator.Validate(dto);

            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing is null)
                throw new KeyNotFoundException($"Category with id {dto.Id} was not found.");

            await _repository.UpdateAsync(MapToEntity(dto));
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                throw new KeyNotFoundException($"Category with id {id} was not found.");

            await _repository.DeleteAsync(id);
        }

        private static CategoryDTO MapToDto(Category c) => new()
        {
            Id          = c.Id,
            Name        = c.Name,
            Description = c.Description
        };

        private static Category MapToEntity(CategoryDTO dto) => new()
        {
            Id          = dto.Id,
            Name        = dto.Name,
            Description = dto.Description
        };
    }
}
