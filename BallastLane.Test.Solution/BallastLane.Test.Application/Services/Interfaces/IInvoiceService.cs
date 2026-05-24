using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDTO>> GetAllAsync();
        Task<InvoiceDTO?> GetByIdAsync(int id);
        Task<IEnumerable<InvoiceDTO>> GetByCustomerIdAsync(int customerId);
        Task<int> CreateAsync(InvoiceDTO dto);
        Task DeleteAsync(int id);
    }
}
