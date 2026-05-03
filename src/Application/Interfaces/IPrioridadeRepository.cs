using SupportTickets.Domain.Entities;

namespace SupportTickets.Application.Interfaces;

public interface IPrioridadeRepository : IRepository<Prioridade>
{
    Task<bool> HasChamadosAsync(int prioridadeId, CancellationToken cancellationToken = default);
}
