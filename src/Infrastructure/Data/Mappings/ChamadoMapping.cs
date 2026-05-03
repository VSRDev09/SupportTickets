using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Infrastructure.Data.Mappings;

public sealed class ChamadoMapping : IEntityTypeConfiguration<Chamado>
{
    // Aqui iriei mapear as Entitdades do projeto pro EF Core saber quem elas serão no banco
    public void Configure(EntityTypeBuilder<Chamado> builder)
    {
        builder.ToTable("chamados");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(x => x.SetorId)
            .HasColumnName("setor_id")
            .IsRequired();

        builder.Property(x => x.PrioridadeId)
            .HasColumnName("prioridade_id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("status_chamado")
            .IsRequired();

        builder.Property(x => x.CriadoEm)
            .HasColumnName("criado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.FinalizadoEm)
            .HasColumnName("finalizado_em")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.FinalizadoPor)
            .HasColumnName("finalizado_por");

        builder.Property(x => x.CanceladoEm)
            .HasColumnName("cancelado_em")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.CanceladoPor)
            .HasColumnName("cancelado_por");

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Setor)
            .WithMany()
            .HasForeignKey(x => x.SetorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Prioridade)
            .WithMany()
            .HasForeignKey(x => x.PrioridadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.FinalizadoPor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.CanceladoPor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Atendimento)
            .WithOne(x => x.Chamado)
            .HasForeignKey<Atendimento>(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Historicos)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Historicos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // regras de negócio do banco
        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_chamados_finalizacao",
                "(status <> 'FINALIZADO' OR (finalizado_em IS NOT NULL AND finalizado_por IS NOT NULL))");

            tableBuilder.HasCheckConstraint(
                "ck_chamados_cancelamento",
                "(status <> 'CANCELADO' OR (cancelado_em IS NOT NULL AND cancelado_por IS NOT NULL))");
        });
    }
}
