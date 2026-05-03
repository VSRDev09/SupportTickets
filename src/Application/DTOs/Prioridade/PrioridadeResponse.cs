namespace SupportTickets.Application.DTOs.Prioridade;

public sealed class PrioridadeResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public int TempoEstimadoHoras { get; init; }
}
