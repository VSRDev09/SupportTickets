using SupportTickets.Application.DTOs.Chamado;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.Interfaces;

public interface IChamadoRepository : IRepository<Chamado>
{
    Task<Chamado?> GetByIdComDetalhesAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Chamado>> ListarComDetalhesAsync(
        ChamadoFiltroRequest filtro,
        PerfilUsuario perfil,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
