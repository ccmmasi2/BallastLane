using BallastLane.Test.Domain.Entities;

namespace BallastLane.Test.Infrastructure.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
        Task<int> CreateAsync(Invoice invoice);
        Task DeleteAsync(int id);
    }
}
