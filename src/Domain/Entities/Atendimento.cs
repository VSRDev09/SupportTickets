using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Domain.Entities;

public class Atendimento
{
    private Atendimento()
    {
    }

    public int Id { get; private set; }
    public int ChamadoId { get; private set; }
    public int AtendenteId { get; private set; }
    public DateTime IniciadoEm { get; private set; }
    public DateTime? FinalizadoEm { get; private set; }
    public string? Solucao { get; private set; }

    public Chamado Chamado { get; private set; } = null!;
    public Usuario Atendente { get; private set; } = null!;

    public bool EstaFinalizado => FinalizadoEm.HasValue;

    public static Atendimento Criar(int atendenteId, DateTime iniciadoEmUtc)
    {
        if (atendenteId <= 0)
        {
            throw new DomainException("O atendente responsável pelo atendimento é obrigatório.");
        }

        return new Atendimento
        {
            AtendenteId = atendenteId,
            IniciadoEm = iniciadoEmUtc
        };
    }

    public void Finalizar(string solucao, DateTime finalizadoEmUtc)
    {
        if (EstaFinalizado)
        {
            throw new DomainException("O atendimento já foi encerrado.");
        }

        if (string.IsNullOrWhiteSpace(solucao))
        {
            throw new DomainException("A solução do atendimento é obrigatória.");
        }

        if (finalizadoEmUtc <= IniciadoEm)
        {
            throw new DomainException("A data de finalização deve ser maior que a data de início do atendimento.");
        }

        FinalizadoEm = finalizadoEmUtc;
        Solucao = solucao.Trim();
    }

    public void EncerrarPorCancelamento(DateTime encerradoEmUtc)
    {
        if (EstaFinalizado)
        {
            return;
        }

        if (encerradoEmUtc <= IniciadoEm)
        {
            throw new DomainException("A data de encerramento do atendimento deve ser maior que a data de início.");
        }

        FinalizadoEm = encerradoEmUtc;
    }

    public TimeSpan ObterTempoTotal(DateTime referenciaUtc)
    {
        var fim = FinalizadoEm ?? referenciaUtc;

        if (fim <= IniciadoEm)
        {
            return TimeSpan.Zero;
        }

        return fim - IniciadoEm;
    }
}
