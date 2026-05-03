using SupportTickets.Application.DTOs.Chamado;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.Interfaces;

public interface IChamadoService
{
    Task<ChamadoResponse> CriarAsync(CriarChamadoRequest request, int usuarioId, CancellationToken cancellationToken = default);

    Task<ChamadoResponse> ObterPorIdAsync(
        int id,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChamadoListItemResponse>> ListarAsync(
        ChamadoFiltroRequest filtro,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default);

    Task<ChamadoResponse> IniciarAsync(int id, int atendenteId, CancellationToken cancellationToken = default);

    Task<ChamadoResponse> FinalizarAsync(
        int id,
        FinalizarChamadoRequest request,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default);

    Task<ChamadoResponse> CancelarAsync(
        int id,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default);

    Task<ChamadoResponse> AlterarPrioridadeAsync(
        int id,
        AtualizarPrioridadeChamadoRequest request,
        CancellationToken cancellationToken = default);
}
