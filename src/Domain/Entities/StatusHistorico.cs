using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Domain.Entities;

public class StatusHistorico
{
    private StatusHistorico()
    {
    }

    public int Id { get; private set; }
    public int ChamadoId { get; private set; }
    public StatusChamado Status { get; private set; }
    public int AlteradoPor { get; private set; }
    public DateTime AlteradoEm { get; private set; }

    public Chamado Chamado { get; private set; } = null!;

    internal static StatusHistorico Criar(StatusChamado status, int alteradoPor, DateTime alteradoEmUtc)
    {
        if (alteradoPor <= 0)
        {
            throw new DomainException("O usuário responsável pela alteração de status é obrigatório.");
        }

        return new StatusHistorico
        {
            Status = status,
            AlteradoPor = alteradoPor,
            AlteradoEm = alteradoEmUtc
        };
    }
}
