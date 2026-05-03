using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class HistoricoStatusResponse
{
    public int Id { get; init; }
    public StatusChamado Status { get; init; }
    public int AlteradoPor { get; init; }
    public DateTime AlteradoEm { get; init; }
}
