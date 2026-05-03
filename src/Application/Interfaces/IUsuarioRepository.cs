using SupportTickets.Domain.Entities;

namespace SupportTickets.Application.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
