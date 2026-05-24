using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Domain.Exceptions;
using BallastLane.Test.Infrastructure.Authentication;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BallastLane.Test.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly PasswordHasher        _passwordHasher;
        private readonly AuthService           _sut;

        public AuthServiceTests()
        {
            _userRepoMock   = new Mock<IUserRepository>();
            _passwordHasher = new PasswordHasher();
            var tokenGenerator = new JwtTokenGenerator(BuildTestConfiguration());

            _sut = new AuthService(
                _userRepoMock.Object,
                _passwordHasher,
                tokenGenerator,
                new RegisterValidator(),
                new LoginValidator());
        }

        private static IConfiguration BuildTestConfiguration() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Secret"]   = "test-super-secret-key-at-least-32-chars!!",
                    ["Jwt:Issuer"]   = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience"
                })
                .Build();

        private static RegisterRequestDTO MakeRegisterDto(
            string username = "johndoe",
            string email    = "john@example.com",
            string password = "password123") => new()
        {
            Username = username,
            Email    = email,
            Password = password
        };

        private static LoginRequestDTO MakeLoginDto(
            string email    = "john@example.com",
            string password = "password123") => new()
        {
            Email    = email,
            Password = password
        };

        // ── RegisterAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task RegisterAsync_WhenDtoIsValid_ReturnsAuthResponseWithToken()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);

            // Act
            var result = await _sut.RegisterAsync(MakeRegisterDto());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("johndoe",           result.Username);
            Assert.Equal("john@example.com",  result.Email);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task RegisterAsync_WhenDtoIsValid_CallsCreateRepositoryOnce()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);

            // Act
            await _sut.RegisterAsync(MakeRegisterDto());

            // Assert
            _userRepoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WhenDtoIsValid_StoresHashedPassword()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            User? captured = null;
            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => captured = u)
                .ReturnsAsync(1);

            // Act
            await _sut.RegisterAsync(MakeRegisterDto(password: "mysecret"));

            // Assert
            Assert.NotNull(captured);
            Assert.NotEqual("mysecret", captured.PasswordHash);
            Assert.Contains('.', captured.PasswordHash); // salt.hash format
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            _userRepoMock
                .Setup(r => r.GetByEmailAsync("john@example.com"))
                .ReturnsAsync(new User { Id = 1, Username = "existing", Email = "john@example.com", PasswordHash = "x.y" });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RegisterAsync(MakeRegisterDto()));
            _userRepoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RegisterAsync_WhenUsernameIsNullOrWhiteSpace_ThrowsValidationException(string? username)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.RegisterAsync(MakeRegisterDto(username: username!)));
            _userRepoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RegisterAsync_WhenEmailIsNullOrWhiteSpace_ThrowsValidationException(string? email)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.RegisterAsync(MakeRegisterDto(email: email!)));
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailHasNoAtSign_ThrowsValidationException()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.RegisterAsync(MakeRegisterDto(email: "invalidemail")));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("12345")]
        public async Task RegisterAsync_WhenPasswordIsTooShort_ThrowsValidationException(string password)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.RegisterAsync(MakeRegisterDto(password: password)));
        }

        // ── LoginAsync ────────────────────────────────────────────────────────

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnsAuthResponseWithToken()
        {
            // Arrange
            var hash = _passwordHasher.Hash("password123");
            _userRepoMock
                .Setup(r => r.GetByEmailAsync("john@example.com"))
                .ReturnsAsync(new User { Id = 1, Username = "johndoe", Email = "john@example.com", PasswordHash = hash });

            // Act
            var result = await _sut.LoginAsync(MakeLoginDto());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("johndoe",          result.Username);
            Assert.Equal("john@example.com", result.Email);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task LoginAsync_WhenEmailDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(MakeLoginDto()));
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsWrong_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var hash = _passwordHasher.Hash("correctpassword");
            _userRepoMock
                .Setup(r => r.GetByEmailAsync("john@example.com"))
                .ReturnsAsync(new User { Id = 1, Username = "johndoe", Email = "john@example.com", PasswordHash = hash });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(MakeLoginDto(password: "wrongpassword")));
        }

        [Fact]
        public async Task LoginAsync_WhenEmailIsWrong_ErrorMessageDoesNotRevealCause()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(MakeLoginDto()));

            // Assert — same generic message for wrong email or wrong password (prevents enumeration)
            Assert.Equal("Invalid email or password.", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task LoginAsync_WhenEmailIsNullOrWhiteSpace_ThrowsValidationException(string? email)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.LoginAsync(MakeLoginDto(email: email!)));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task LoginAsync_WhenPasswordIsNullOrWhiteSpace_ThrowsValidationException(string? password)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.LoginAsync(MakeLoginDto(password: password!)));
        }
    }
}
