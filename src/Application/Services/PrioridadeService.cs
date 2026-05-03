using SupportTickets.Application.DTOs.Prioridade;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Application.Services;

public sealed class PrioridadeService : IPrioridadeService
{
    private readonly IPrioridadeRepository _prioridadeRepository;

    public PrioridadeService(IPrioridadeRepository prioridadeRepository)
    {
        _prioridadeRepository = prioridadeRepository;
    }

    public async Task<IReadOnlyList<PrioridadeResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var prioridades = await _prioridadeRepository.GetAllAsync(cancellationToken);
        return prioridades.Select(Map).ToList();
    }

    public async Task<PrioridadeResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var prioridade = await _prioridadeRepository.GetByIdAsync(id, cancellationToken)
                         ?? throw new NotFoundException("Prioridade não encontrada.");

        return Map(prioridade);
    }

    public async Task<PrioridadeResponse> CriarAsync(PrioridadeRequest request, CancellationToken cancellationToken = default)
    {
        var prioridade = Prioridade.Criar(request.Nome, request.TempoEstimadoHoras);

        await _prioridadeRepository.AddAsync(prioridade, cancellationToken);
        await _prioridadeRepository.SaveChangesAsync(cancellationToken);

        return Map(prioridade);
    }

    public async Task<PrioridadeResponse> AtualizarAsync(int id, PrioridadeRequest request, CancellationToken cancellationToken = default)
    {
        var prioridade = await _prioridadeRepository.GetByIdAsync(id, cancellationToken)
                         ?? throw new NotFoundException("Prioridade não encontrada.");

        prioridade.Atualizar(request.Nome, request.TempoEstimadoHoras);
        _prioridadeRepository.Update(prioridade);
        await _prioridadeRepository.SaveChangesAsync(cancellationToken);

        return Map(prioridade);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var prioridade = await _prioridadeRepository.GetByIdAsync(id, cancellationToken)
                         ?? throw new NotFoundException("Prioridade não encontrada.");

        if (await _prioridadeRepository.HasChamadosAsync(id, cancellationToken))
        {
            throw new DomainException("Não é possível remover a prioridade porque existem chamados vinculados a ela.");
        }

        _prioridadeRepository.Remove(prioridade);
        await _prioridadeRepository.SaveChangesAsync(cancellationToken);
    }

    private static PrioridadeResponse Map(Prioridade prioridade)
    {
        return new PrioridadeResponse
        {
            Id = prioridade.Id,
            Nome = prioridade.Nome,
            TempoEstimadoHoras = prioridade.TempoEstimadoHoras
        };
    }
}
