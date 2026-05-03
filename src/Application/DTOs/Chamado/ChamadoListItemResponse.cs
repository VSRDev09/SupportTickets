using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class ChamadoListItemResponse
{
    public int Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Setor { get; init; } = string.Empty;
    public string Prioridade { get; init; } = string.Empty;
    public StatusChamado Status { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime? IniciadoEm { get; init; }
    public DateTime? FinalizadoEm { get; init; }
    public string Solicitante { get; init; } = string.Empty;
    public string? Atendente { get; init; }
    public string TempoTotalAtendimento { get; init; } = "00:00:00";
    public double TempoTotalAtendimentoHoras { get; init; }
    public int SlaHoras { get; init; }
    public bool EstaAtrasado { get; init; }
}
