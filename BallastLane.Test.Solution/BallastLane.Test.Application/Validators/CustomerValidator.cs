using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Validators
{
    public class CustomerValidator
    {
        public void Validate(CustomerDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("Customer full name is required.", nameof(dto.FullName));

            if (dto.Email is not null && !dto.Email.Contains('@'))
                throw new ArgumentException("A valid email address is required.", nameof(dto.Email));
        }
    }
}
