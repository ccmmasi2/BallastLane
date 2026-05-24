using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Authentication;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;

namespace BallastLane.Test.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository   _userRepository;
        private readonly PasswordHasher    _passwordHasher;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly RegisterValidator _registerValidator;
        private readonly LoginValidator    _loginValidator;

        public AuthService(
            IUserRepository   userRepository,
            PasswordHasher    passwordHasher,
            JwtTokenGenerator tokenGenerator,
            RegisterValidator registerValidator,
            LoginValidator    loginValidator)
        {
            _userRepository    = userRepository;
            _passwordHasher    = passwordHasher;
            _tokenGenerator    = tokenGenerator;
            _registerValidator = registerValidator;
            _loginValidator    = loginValidator;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO dto)
        {
            _registerValidator.Validate(dto);

            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing is not null)
                throw new InvalidOperationException($"An account with email '{dto.Email}' already exists.");

            var user = new User
            {
                Username     = dto.Username,
                Email        = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password)
            };

            user.Id = await _userRepository.CreateAsync(user);

            return new AuthResponseDTO
            {
                Token    = _tokenGenerator.GenerateToken(user),
                Username = user.Username,
                Email    = user.Email
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO dto)
        {
            _loginValidator.Validate(dto);

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user is null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return new AuthResponseDTO
            {
                Token    = _tokenGenerator.GenerateToken(user),
                Username = user.Username,
                Email    = user.Email
            };
        }
    }
}
