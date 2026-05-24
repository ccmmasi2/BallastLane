using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllAsync();
        Task<CustomerDTO?> GetByIdAsync(int id);
        Task<int> CreateAsync(CustomerDTO dto);
        Task UpdateAsync(CustomerDTO dto);
        Task DeleteAsync(int id);
    }
}
