using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Application.Validators
{
    public class RegisterValidator
    {
        public void Validate(RegisterRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ValidationException("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email is required.");

            if (!dto.Email.Contains('@'))
                throw new ValidationException("A valid email address is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password is required.");

            if (dto.Password.Length < 6)
                throw new ValidationException("Password must be at least 6 characters long.");
        }
    }
}
