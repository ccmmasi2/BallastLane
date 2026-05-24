using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task<int> CreateAsync(CategoryDTO dto);
        Task UpdateAsync(CategoryDTO dto);
        Task DeleteAsync(int id);
    }
}
