using Microsoft.EntityFrameworkCore;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Infrastructure.Data;

namespace SupportTickets.Infrastructure.Repositories;

public sealed class PrioridadeRepository : Repository<Prioridade>, IPrioridadeRepository
{
    public PrioridadeRepository(AppDbContext context) : base(context)
    {
    }

    // Aqui eu identifico se há algum chamado com essa prioridade
    public async Task<bool> HasChamadosAsync(int prioridadeId, CancellationToken cancellationToken = default)
    {
        return await Context.Chamados.AnyAsync(x => x.PrioridadeId == prioridadeId, cancellationToken);
    }
}
