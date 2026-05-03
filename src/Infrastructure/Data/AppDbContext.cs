using Microsoft.EntityFrameworkCore;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Infrastructure.Data;

public class AppDbContext : DbContext
{
    //esse construtor está recebendo configurações do Program.cs
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Setor> Setores => Set<Setor>();
    public DbSet<Prioridade> Prioridades => Set<Prioridade>();
    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<Atendimento> ChamadoAtendimentos => Set<Atendimento>();
    public DbSet<StatusHistorico> ChamadoStatusHistoricos => Set<StatusHistorico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //aqui estou sinalizando pro EF Core que essas classes são Enums e não tipos
        modelBuilder.HasPostgresEnum<StatusChamado>();
        modelBuilder.HasPostgresEnum<PerfilUsuario>();
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
