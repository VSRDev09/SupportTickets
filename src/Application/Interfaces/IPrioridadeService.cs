using SupportTickets.Application.DTOs.Prioridade;

namespace SupportTickets.Application.Interfaces;

public interface IPrioridadeService
{
    Task<IReadOnlyList<PrioridadeResponse>> ListarAsync(CancellationToken cancellationToken = default);
    Task<PrioridadeResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PrioridadeResponse> CriarAsync(PrioridadeRequest request, CancellationToken cancellationToken = default);
    Task<PrioridadeResponse> AtualizarAsync(int id, PrioridadeRequest request, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
