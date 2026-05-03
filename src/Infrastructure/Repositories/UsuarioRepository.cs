using Microsoft.EntityFrameworkCore;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Infrastructure.Data;

namespace SupportTickets.Infrastructure.Repositories;

public sealed class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Usuarios
            .Include(x => x.Setor)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Usuarios
            .Include(x => x.Setor)
            .FirstOrDefaultAsync(x => x.Email == email.Trim().ToLower(), cancellationToken);
    }
}
