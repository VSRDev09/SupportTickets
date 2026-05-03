using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTickets.Domain.Entities;

namespace SupportTickets.Infrastructure.Data.Mappings;

public sealed class AtendimentoMapping : IEntityTypeConfiguration<Atendimento>
{
    public void Configure(EntityTypeBuilder<Atendimento> builder)
    {
        builder.ToTable("chamado_atendimentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ChamadoId)
            .HasColumnName("chamado_id")
            .IsRequired();

        builder.Property(x => x.AtendenteId)
            .HasColumnName("atendente_id")
            .IsRequired();

        builder.Property(x => x.IniciadoEm)
            .HasColumnName("iniciado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.FinalizadoEm)
            .HasColumnName("finalizado_em")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.Solucao)
            .HasColumnName("solucao")
            .HasColumnType("text");

        builder.HasIndex(x => x.ChamadoId)
            .IsUnique();

        builder.HasOne(x => x.Atendente)
            .WithMany()
            .HasForeignKey(x => x.AtendenteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_chamado_atendimentos_datas",
                "(finalizado_em IS NULL OR finalizado_em > iniciado_em)");
        });
    }
}
