using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Application.Validators
{
    public class InvoiceValidator
    {
        public void Validate(InvoiceDTO dto)
        {
            if (dto.CustomerId <= 0)
                throw new ValidationException("A valid customer is required.");

            if (dto.CreatedByUserId <= 0)
                throw new ValidationException("A valid user is required.");

            if (dto.Details is null || dto.Details.Count == 0)
                throw new ValidationException("Invoice must contain at least one detail line.");

            foreach (var detail in dto.Details)
            {
                if (detail.ProductId <= 0)
                    throw new ValidationException("Each detail line must reference a valid product.");

                if (detail.Quantity <= 0)
                    throw new ValidationException("Quantity must be greater than zero.");

                if (detail.UnitPrice <= 0)
                    throw new ValidationException("Unit price must be greater than zero.");
            }
        }
    }
}
