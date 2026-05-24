using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Domain.Exceptions;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;

namespace BallastLane.Test.Application.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository  _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly InvoiceValidator    _validator;

        public InvoiceService(
            IInvoiceRepository  invoiceRepository,
            ICustomerRepository customerRepository,
            InvoiceValidator    validator)
        {
            _invoiceRepository  = invoiceRepository;
            _customerRepository = customerRepository;
            _validator          = validator;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return invoices.Select(MapToDto);
        }

        public async Task<InvoiceDTO?> GetByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            return invoice is null ? null : MapToDto(invoice);
        }

        public async Task<IEnumerable<InvoiceDTO>> GetByCustomerIdAsync(int customerId)
        {
            var invoices = await _invoiceRepository.GetByCustomerIdAsync(customerId);
            return invoices.Select(MapToDto);
        }

        public async Task<int> CreateAsync(InvoiceDTO dto)
        {
            _validator.Validate(dto);

            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer is null)
                throw new NotFoundException($"Customer with id {dto.CustomerId} was not found.");

            // Snapshot customer data at the moment of invoicing so the record
            // remains accurate even if the customer is edited later.
            dto.CustomerFullName       = customer.FullName;
            dto.CustomerDocumentNumber = customer.DocumentNumber;
            dto.CustomerEmail          = customer.Email;
            dto.CustomerPhone          = customer.Phone;
            dto.CustomerAddress        = customer.Address;

            // Recalculate to prevent tampered totals coming from the client.
            foreach (var detail in dto.Details)
                detail.Subtotal = detail.UnitPrice * detail.Quantity;

            dto.Total       = dto.Details.Sum(d => d.Subtotal);
            dto.InvoiceDate = DateTime.UtcNow;

            return await _invoiceRepository.CreateAsync(MapToEntity(dto));
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _invoiceRepository.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException($"Invoice with id {id} was not found.");

            await _invoiceRepository.DeleteAsync(id);
        }

        // ── Mapping ──────────────────────────────────────────────────────────

        private static InvoiceDTO MapToDto(Invoice i) => new()
        {
            Id                     = i.Id,
            InvoiceDate            = i.InvoiceDate,
            Total                  = i.Total,
            CreatedByUserId        = i.CreatedByUserId,
            CustomerId             = i.CustomerId,
            CustomerFullName       = i.CustomerFullName,
            CustomerDocumentNumber = i.CustomerDocumentNumber,
            CustomerEmail          = i.CustomerEmail,
            CustomerPhone          = i.CustomerPhone,
            CustomerAddress        = i.CustomerAddress,
            Details                = i.Details.Select(MapDetailToDto).ToList()
        };

        private static InvoiceDetailDTO MapDetailToDto(InvoiceDetail d) => new()
        {
            Id           = d.Id,
            ProductId    = d.ProductId,
            ProductName  = d.ProductName,
            CategoryName = d.CategoryName,
            UnitPrice    = d.UnitPrice,
            Quantity     = d.Quantity,
            Subtotal     = d.Subtotal
        };

        private static Invoice MapToEntity(InvoiceDTO dto) => new()
        {
            Id                     = dto.Id,
            InvoiceDate            = dto.InvoiceDate,
            Total                  = dto.Total,
            CreatedByUserId        = dto.CreatedByUserId,
            CustomerId             = dto.CustomerId,
            CustomerFullName       = dto.CustomerFullName,
            CustomerDocumentNumber = dto.CustomerDocumentNumber,
            CustomerEmail          = dto.CustomerEmail,
            CustomerPhone          = dto.CustomerPhone,
            CustomerAddress        = dto.CustomerAddress,
            Details                = dto.Details.Select(MapDetailToEntity).ToList()
        };

        private static InvoiceDetail MapDetailToEntity(InvoiceDetailDTO dto) => new()
        {
            Id           = dto.Id,
            ProductId    = dto.ProductId,
            ProductName  = dto.ProductName,
            CategoryName = dto.CategoryName,
            UnitPrice    = dto.UnitPrice,
            Quantity     = dto.Quantity,
            Subtotal     = dto.Subtotal
        };
    }
}
