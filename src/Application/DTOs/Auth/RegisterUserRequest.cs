using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.DTOs.Auth;

public sealed class RegisterUserRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public int SetorId { get; set; }
}
