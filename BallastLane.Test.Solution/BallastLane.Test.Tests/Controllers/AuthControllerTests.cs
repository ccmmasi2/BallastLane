using BallastLane.Test.API.Controllers;
using BallastLane.Test.Application.Common;
using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BallastLane.Test.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _serviceMock;
        private readonly AuthController    _sut;

        public AuthControllerTests()
        {
            _serviceMock = new Mock<IAuthService>();
            _sut         = new AuthController(_serviceMock.Object);
        }

        private static AuthResponseDTO MakeAuthResponse() => new()
        {
            Token    = "eyJhbGciOiJIUzI1NiJ9.test.signature",
            Username = "johndoe",
            Email    = "john@example.com"
        };

        // ── Register ─────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_WhenDtoIsValid_ReturnsOkWithToken()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDTO>()))
                .ReturnsAsync(MakeAuthResponse());

            // Act
            var result   = await _sut.Register(new RegisterRequestDTO { Username = "johndoe", Email = "john@example.com", Password = "pass123" });
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<AuthResponseDTO>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("johndoe",          response.Data.Username);
            Assert.Equal("john@example.com", response.Data.Email);
            Assert.NotEmpty(response.Data.Token);
        }

        [Fact]
        public async Task Register_WhenDtoIsValid_ReturnsSuccessMessage()
        {
            // Arrange
            _serviceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDTO>())).ReturnsAsync(MakeAuthResponse());

            // Act
            var result   = await _sut.Register(new RegisterRequestDTO());
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<AuthResponseDTO>>(ok.Value);

            // Assert
            Assert.Contains("registration", response.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Register_WhenDtoIsValid_CallsServiceOnce()
        {
            // Arrange
            _serviceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDTO>())).ReturnsAsync(MakeAuthResponse());

            // Act
            await _sut.Register(new RegisterRequestDTO());

            // Assert
            _serviceMock.Verify(s => s.RegisterAsync(It.IsAny<RegisterRequestDTO>()), Times.Once);
        }

        // ── Login ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_WhenCredentialsAreValid_ReturnsOkWithToken()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDTO>()))
                .ReturnsAsync(MakeAuthResponse());

            // Act
            var result   = await _sut.Login(new LoginRequestDTO { Email = "john@example.com", Password = "pass" });
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<AuthResponseDTO>>(ok.Value);

            // Assert
            Assert.True(response.Success);
            Assert.NotEmpty(response.Data.Token);
        }

        [Fact]
        public async Task Login_WhenCredentialsAreValid_ReturnsLoginSuccessMessage()
        {
            // Arrange
            _serviceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDTO>())).ReturnsAsync(MakeAuthResponse());

            // Act
            var result   = await _sut.Login(new LoginRequestDTO());
            var ok       = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<AuthResponseDTO>>(ok.Value);

            // Assert
            Assert.Contains("login", response.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Login_WhenCredentialsAreValid_CallsServiceOnce()
        {
            // Arrange
            _serviceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDTO>())).ReturnsAsync(MakeAuthResponse());

            // Act
            await _sut.Login(new LoginRequestDTO());

            // Assert
            _serviceMock.Verify(s => s.LoginAsync(It.IsAny<LoginRequestDTO>()), Times.Once);
        }
    }
}
