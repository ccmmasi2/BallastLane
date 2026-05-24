using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDTO>> GetByCategoryIdAsync(int categoryId);
        Task<int> CreateAsync(ProductDTO dto);
        Task UpdateAsync(ProductDTO dto);
        Task DeleteAsync(int id);
    }
}
