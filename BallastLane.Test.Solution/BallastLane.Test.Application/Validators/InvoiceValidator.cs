using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Validators
{
    public class InvoiceValidator
    {
        public void Validate(InvoiceDTO dto)
        {
            if (dto.CustomerId <= 0)
                throw new ArgumentException("A valid customer is required.", nameof(dto.CustomerId));

            if (dto.CreatedByUserId <= 0)
                throw new ArgumentException("A valid user is required.", nameof(dto.CreatedByUserId));

            if (dto.Details is null || dto.Details.Count == 0)
                throw new ArgumentException("Invoice must contain at least one detail line.", nameof(dto.Details));

            foreach (var detail in dto.Details)
            {
                if (detail.ProductId <= 0)
                    throw new ArgumentException("Each detail line must reference a valid product.");

                if (detail.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero.");

                if (detail.UnitPrice <= 0)
                    throw new ArgumentException("Unit price must be greater than zero.");
            }
        }
    }
}
