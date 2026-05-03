using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.DTOs.Auth;

public sealed class UserResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public PerfilUsuario Perfil { get; init; }
    public int SetorId { get; init; }
    public string? SetorNome { get; init; }
    public bool Ativo { get; init; }
}
