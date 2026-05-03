using SupportTickets.Domain.Entities;

namespace SupportTickets.Application.Interfaces;

public interface ISetorRepository : IRepository<Setor>
{
    Task<bool> HasVinculosAsync(int setorId, CancellationToken cancellationToken = default);
}
