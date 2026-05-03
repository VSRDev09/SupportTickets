using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Infrastructure.Data.Mappings;

public sealed class StatusHistoricoMapping : IEntityTypeConfiguration<StatusHistorico>
{
    public void Configure(EntityTypeBuilder<StatusHistorico> builder)
    {
        builder.ToTable("chamado_status_historico");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ChamadoId)
            .HasColumnName("chamado_id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("status_chamado")
            .IsRequired();

        builder.Property(x => x.AlteradoPor)
            .HasColumnName("alterado_por")
            .IsRequired();

        builder.Property(x => x.AlteradoEm)
            .HasColumnName("alterado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.AlteradoPor)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
