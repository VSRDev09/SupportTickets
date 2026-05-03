using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class ChamadoFiltroRequest
{
    public StatusChamado? Status { get; set; }
    public int? SetorId { get; set; }
    public int? PrioridadeId { get; set; }
    public bool ApenasAtrasados { get; set; }
    public string OrdenarPor { get; set; } = "criadoEm_desc";
}
