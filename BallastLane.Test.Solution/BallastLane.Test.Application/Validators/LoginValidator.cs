using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Validators
{
    public class LoginValidator
    {
        public void Validate(LoginRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.", nameof(dto.Email));

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.", nameof(dto.Password));
        }
    }
}
