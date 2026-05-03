using Microsoft.EntityFrameworkCore;
using SupportTickets.Application.DTOs.Chamado;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;
using SupportTickets.Infrastructure.Data;

namespace SupportTickets.Infrastructure.Repositories;

public sealed class ChamadoRepository : Repository<Chamado>, IChamadoRepository
{
    public ChamadoRepository(AppDbContext context) : base(context)
    {
        //Aqui estou passando a instância do AppDbContext para o construtor da classe base (Repository)
    }

    public async Task<Chamado?> GetByIdComDetalhesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await BaseQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <summary>
    /// Aqui eu estou listando chamados com detalhes, aplicando:
    /// - Controle de acesso por perfil
    /// - Filtros opcionais (status, setor, prioridade)
    /// - Ordenação dinâmica
    ///
    /// Obs: consulta somente leitura (AsNoTracking) e com includes via BaseQuery.
    /// </summary>
    public async Task<IReadOnlyList<Chamado>> ListarComDetalhesAsync(
        ChamadoFiltroRequest filtro,
        PerfilUsuario perfil,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Chamado> query = BaseQuery().AsNoTracking();

        // se o usuário tiver perfil de usuário (não é atendente sem admin)
        // mostra só os chamados desse usuário
        if (perfil == PerfilUsuario.Usuario)
        {
            query = query.Where(x => x.UsuarioId == usuarioId);
        }

        //aqui vai filtrar por esses valores
        if (filtro.Status.HasValue)
        {
            query = query.Where(x => x.Status == filtro.Status.Value);
        }

        if (filtro.SetorId.HasValue)
        {
            query = query.Where(x => x.SetorId == filtro.SetorId.Value);
        }

        if (filtro.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadeId == filtro.PrioridadeId.Value);
        }

        query = filtro.OrdenarPor switch
        {
            "criadoEm_asc" => query.OrderBy(x => x.CriadoEm),
            "prioridade_asc" => query.OrderBy(x => x.Prioridade.Nome),
            "prioridade_desc" => query.OrderByDescending(x => x.Prioridade.Nome),
            "status_asc" => query.OrderBy(x => x.Status),
            "status_desc" => query.OrderByDescending(x => x.Status),
            _ => query.OrderByDescending(x => x.CriadoEm)
        };

        return await query.ToListAsync(cancellationToken);// cancelation permite o cancelamento da query
    }

    // aqui eu permito que a query seja feita com todos os dados relacionados
    private IQueryable<Chamado> BaseQuery()
    {
        return Context.Chamados
            .Include(x => x.Usuario)
            .ThenInclude(x => x.Setor)
            .Include(x => x.Setor)
            .Include(x => x.Prioridade)
            .Include(x => x.Atendimento!)
            .ThenInclude(x => x.Atendente)
            .Include(x => x.Historicos);
    }
}
