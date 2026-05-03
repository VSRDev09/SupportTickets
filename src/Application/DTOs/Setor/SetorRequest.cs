namespace SupportTickets.Application.DTOs.Setor;

public sealed class SetorRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
