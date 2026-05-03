using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTickets.Domain.Entities;

namespace SupportTickets.Infrastructure.Data.Mappings;

public sealed class PrioridadeMapping : IEntityTypeConfiguration<Prioridade>
{
    public void Configure(EntityTypeBuilder<Prioridade> builder)
    {
        builder.ToTable("prioridades");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TempoEstimadoHoras)
            .HasColumnName("tempo_estimado_horas")
            .IsRequired();

        builder.HasIndex(x => x.Nome)
            .IsUnique();

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_prioridades_tempo_estimado_horas", "tempo_estimado_horas > 0");
        });
    }
}
