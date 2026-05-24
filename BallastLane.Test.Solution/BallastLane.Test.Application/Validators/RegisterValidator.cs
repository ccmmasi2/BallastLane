using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Validators
{
    public class RegisterValidator
    {
        public void Validate(RegisterRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("Username is required.", nameof(dto.Username));

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.", nameof(dto.Email));

            if (!dto.Email.Contains('@'))
                throw new ArgumentException("A valid email address is required.", nameof(dto.Email));

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.", nameof(dto.Password));

            if (dto.Password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(dto.Password));
        }
    }
}
