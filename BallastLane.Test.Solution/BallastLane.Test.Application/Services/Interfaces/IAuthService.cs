using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO dto);
    }
}
