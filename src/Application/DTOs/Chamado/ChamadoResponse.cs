using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class ChamadoResponse
{
    public int Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public int UsuarioId { get; init; }
    public string Solicitante { get; init; } = string.Empty;
    public int SetorId { get; init; }
    public string Setor { get; init; } = string.Empty;
    public int PrioridadeId { get; init; }
    public string Prioridade { get; init; } = string.Empty;
    public int SlaHoras { get; init; }
    public StatusChamado Status { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime? IniciadoEm { get; init; }
    public int? AtendenteId { get; init; }
    public string? Atendente { get; init; }
    public DateTime? FinalizadoEm { get; init; }
    public int? FinalizadoPor { get; init; }
    public DateTime? CanceladoEm { get; init; }
    public int? CanceladoPor { get; init; }
    public string? Solucao { get; init; }
    public string TempoTotalAtendimento { get; init; } = "00:00:00";
    public double TempoTotalAtendimentoHoras { get; init; }
    public bool EstaAtrasado { get; init; }
    public IReadOnlyList<HistoricoStatusResponse> HistoricoStatus { get; init; } = [];
}
