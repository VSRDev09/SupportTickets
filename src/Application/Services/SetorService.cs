using SupportTickets.Application.DTOs.Setor;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Application.Services;

public sealed class SetorService : ISetorService
{
    private readonly ISetorRepository _setorRepository;

    public SetorService(ISetorRepository setorRepository)
    {
        _setorRepository = setorRepository;
    }

    public async Task<IReadOnlyList<SetorResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var setores = await _setorRepository.GetAllAsync(cancellationToken);
        return setores.Select(Map).ToList();
    }

    public async Task<SetorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var setor = await _setorRepository.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException("Setor não encontrado.");

        return Map(setor);
    }

    public async Task<SetorResponse> CriarAsync(SetorRequest request, CancellationToken cancellationToken = default)
    {
        var setor = Setor.Criar(request.Nome, request.Descricao);

        await _setorRepository.AddAsync(setor, cancellationToken);
        await _setorRepository.SaveChangesAsync(cancellationToken);

        return Map(setor);
    }

    public async Task<SetorResponse> AtualizarAsync(int id, SetorRequest request, CancellationToken cancellationToken = default)
    {
        var setor = await _setorRepository.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException("Setor não encontrado.");

        setor.Atualizar(request.Nome, request.Descricao);
        _setorRepository.Update(setor);
        await _setorRepository.SaveChangesAsync(cancellationToken);

        return Map(setor);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var setor = await _setorRepository.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException("Setor não encontrado.");

        if (await _setorRepository.HasVinculosAsync(id, cancellationToken))
        {
            throw new DomainException("Não é possível remover o setor porque existem usuários ou chamados vinculados a ele.");
        }

        _setorRepository.Remove(setor);
        await _setorRepository.SaveChangesAsync(cancellationToken);
    }

    private static SetorResponse Map(Setor setor)
    {
        return new SetorResponse
        {
            Id = setor.Id,
            Nome = setor.Nome,
            Descricao = setor.Descricao
        };
    }
}
