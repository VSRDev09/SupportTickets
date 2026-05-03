namespace SupportTickets.Application.DTOs.Setor;

public sealed class SetorResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
}
