using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;

namespace BallastLane.Test.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly CustomerValidator   _validator;

        public CustomerService(ICustomerRepository repository, CustomerValidator validator)
        {
            _repository = repository;
            _validator  = validator;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers.Select(MapToDto);
        }

        public async Task<CustomerDTO?> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return customer is null ? null : MapToDto(customer);
        }

        public async Task<int> CreateAsync(CustomerDTO dto)
        {
            _validator.Validate(dto);
            return await _repository.CreateAsync(MapToEntity(dto));
        }

        public async Task UpdateAsync(CustomerDTO dto)
        {
            _validator.Validate(dto);

            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing is null)
                throw new KeyNotFoundException($"Customer with id {dto.Id} was not found.");

            await _repository.UpdateAsync(MapToEntity(dto));
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                throw new KeyNotFoundException($"Customer with id {id} was not found.");

            await _repository.DeleteAsync(id);
        }

        private static CustomerDTO MapToDto(Customer c) => new()
        {
            Id             = c.Id,
            FullName       = c.FullName,
            DocumentNumber = c.DocumentNumber,
            Email          = c.Email,
            Phone          = c.Phone,
            Address        = c.Address
        };

        private static Customer MapToEntity(CustomerDTO dto) => new()
        {
            Id             = dto.Id,
            FullName       = dto.FullName,
            DocumentNumber = dto.DocumentNumber,
            Email          = dto.Email,
            Phone          = dto.Phone,
            Address        = dto.Address
        };
    }
}
