using SupportTickets.Application.DTOs.Auth;

namespace SupportTickets.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
}
