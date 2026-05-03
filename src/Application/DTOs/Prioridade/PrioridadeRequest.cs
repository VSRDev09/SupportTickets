namespace SupportTickets.Application.DTOs.Prioridade;

public sealed class PrioridadeRequest
{
    public string Nome { get; set; } = string.Empty;
    public int TempoEstimadoHoras { get; set; }
}
