using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Application.Validators
{
    public class CustomerValidator
    {
        public void Validate(CustomerDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ValidationException("Customer full name is required.");

            if (dto.Email is not null && !dto.Email.Contains('@'))
                throw new ValidationException("A valid email address is required.");
        }
    }
}
