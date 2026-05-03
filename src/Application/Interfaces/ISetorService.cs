using SupportTickets.Application.DTOs.Setor;

namespace SupportTickets.Application.Interfaces;

public interface ISetorService
{
    Task<IReadOnlyList<SetorResponse>> ListarAsync(CancellationToken cancellationToken = default);
    Task<SetorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SetorResponse> CriarAsync(SetorRequest request, CancellationToken cancellationToken = default);
    Task<SetorResponse> AtualizarAsync(int id, SetorRequest request, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
