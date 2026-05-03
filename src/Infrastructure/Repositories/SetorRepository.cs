using Microsoft.EntityFrameworkCore;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Infrastructure.Data;

namespace SupportTickets.Infrastructure.Repositories;

public sealed class SetorRepository : Repository<Setor>, ISetorRepository
{
    public SetorRepository(AppDbContext context) : base(context)
    {
    }

    // vai retornar verdadeiro se tiver algum usuário ou algum chamado vinculados a esse setor
    public async Task<bool> HasVinculosAsync(int setorId, CancellationToken cancellationToken = default)
    {
        return await Context.Usuarios.AnyAsync(x => x.SetorId == setorId, cancellationToken)
               || await Context.Chamados.AnyAsync(x => x.SetorId == setorId, cancellationToken);
    }
}
