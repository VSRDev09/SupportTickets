using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Domain.Entities;

public class Chamado
{
    private readonly List<StatusHistorico> _historicos = [];

    private Chamado()
    {
    }

    public int Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public int UsuarioId { get; private set; }
    public int SetorId { get; private set; }
    public int PrioridadeId { get; private set; }
    public StatusChamado Status { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? FinalizadoEm { get; private set; }
    public int? FinalizadoPor { get; private set; }
    public DateTime? CanceladoEm { get; private set; }
    public int? CanceladoPor { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Setor Setor { get; private set; } = null!;
    public Prioridade Prioridade { get; private set; } = null!;
    public Atendimento? Atendimento { get; private set; }

    //Quero que histórico seja imutável apenas para auditoria
    public IReadOnlyCollection<StatusHistorico> Historicos => _historicos.AsReadOnly();

    //Aqui irei criar o chamado aplicando as validações de domínio
    public static Chamado Criar(
        string titulo,
        string descricao,
        int usuarioId,
        int setorId,
        int prioridadeId,
        DateTime criadoEmUtc)
    {
        ValidarDadosBasicos(titulo, descricao, usuarioId, setorId, prioridadeId);

        var chamado = new Chamado
        {
            Titulo = titulo.Trim(),
            Descricao = descricao.Trim(),
            UsuarioId = usuarioId,
            SetorId = setorId,
            PrioridadeId = prioridadeId,
            Status = StatusChamado.Aberto,
            CriadoEm = criadoEmUtc
        };

        chamado.AdicionarHistorico(StatusChamado.Aberto, usuarioId, criadoEmUtc);

        return chamado;
    }

    public void IniciarAtendimento(int atendenteId, DateTime iniciadoEmUtc)
    {
        if (Status != StatusChamado.Aberto)
        {
            throw new DomainException("Somente chamados com status Aberto podem ser iniciados.");
        }

        if (Atendimento is not null)
        {
            throw new DomainException("Este chamado já possui um atendimento iniciado.");
        }

        Atendimento = Atendimento.Criar(atendenteId, iniciadoEmUtc);
        Status = StatusChamado.Executando;

        AdicionarHistorico(StatusChamado.Executando, atendenteId, iniciadoEmUtc);
    }

    public void Finalizar(int finalizadoPor, string solucao, DateTime finalizadoEmUtc)
    {
        if (Status != StatusChamado.Executando)
        {
            throw new DomainException("Somente chamados em execução podem ser finalizados.");
        }

        if (Atendimento is null)
        {
            throw new DomainException("Não é possível finalizar um chamado sem atendimento iniciado.");
        }

        Atendimento.Finalizar(solucao, finalizadoEmUtc);

        Status = StatusChamado.Finalizado;
        FinalizadoEm = finalizadoEmUtc;
        FinalizadoPor = finalizadoPor;

        AdicionarHistorico(StatusChamado.Finalizado, finalizadoPor, finalizadoEmUtc);
    }

    public void Cancelar(int canceladoPor, DateTime canceladoEmUtc)
    {
        if (Status == StatusChamado.Finalizado)
        {
            throw new DomainException("Não é possível cancelar um chamado finalizado.");
        }

        if (Status == StatusChamado.Cancelado)
        {
            throw new DomainException("Não é possível cancelar um chamado que já foi cancelado.");
        }

        if (Atendimento is not null && !Atendimento.EstaFinalizado)
        {
            Atendimento.EncerrarPorCancelamento(canceladoEmUtc);
        }

        Status = StatusChamado.Cancelado;
        CanceladoEm = canceladoEmUtc;
        CanceladoPor = canceladoPor;

        AdicionarHistorico(StatusChamado.Cancelado, canceladoPor, canceladoEmUtc);
    }

    public void AlterarPrioridade(int prioridadeId)
    {
        if (prioridadeId <= 0)
        {
            throw new DomainException("A prioridade informada é inválida.");
        }

        if (Status is StatusChamado.Finalizado or StatusChamado.Cancelado)
        {
            throw new DomainException("Não é possível alterar a prioridade de chamados finalizados ou cancelados.");
        }

        PrioridadeId = prioridadeId;
    }

    public TimeSpan ObterTempoTotalAtendimento(DateTime referenciaUtc)
    {
        return Atendimento?.ObterTempoTotal(referenciaUtc) ?? TimeSpan.Zero;
    }

    public bool FoiAbertoPor(int usuarioId)
    {
        return UsuarioId == usuarioId;
    }

    public bool AtendimentoPertenceAo(int atendenteId)
    {
        return Atendimento?.AtendenteId == atendenteId;
    }

    private void AdicionarHistorico(StatusChamado status, int alteradoPor, DateTime alteradoEmUtc)
    {
        _historicos.Add(StatusHistorico.Criar(status, alteradoPor, alteradoEmUtc));
    }

    //Aqui estou validando os dados do domínio
    private static void ValidarDadosBasicos(
        string titulo,
        string descricao,
        int usuarioId,
        int setorId,
        int prioridadeId)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new DomainException("O título do chamado é obrigatório.");
        }

        if (titulo.Trim().Length > 200)
        {
            throw new DomainException("O título do chamado deve ter no máximo 200 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new DomainException("A descrição do chamado é obrigatória.");
        }

        if (usuarioId <= 0)
        {
            throw new DomainException("O solicitante do chamado é obrigatório.");
        }

        if (setorId <= 0)
        {
            throw new DomainException("O setor do chamado é obrigatório.");
        }

        if (prioridadeId <= 0)
        {
            throw new DomainException("A prioridade do chamado é obrigatória.");
        }
    }
}
