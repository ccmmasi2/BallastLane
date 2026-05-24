using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Application.Validators
{
    public class LoginValidator
    {
        public void Validate(LoginRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password is required.");
        }
    }
}
