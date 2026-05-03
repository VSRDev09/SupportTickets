namespace SupportTickets.Application.DTOs.Auth;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiraEmUtc { get; init; }
    public UserResponse Usuario { get; init; } = new();
}
