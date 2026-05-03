using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Domain.Tests;

public sealed class ChamadoTests
{
    [Fact]
    public void Criar_DeveIniciarComoAbertoERegistrarHistoricoInicial()
    {
        var chamado = Chamado.Criar(
            "Erro no sistema",
            "Nao consigo acessar o modulo financeiro.",
            usuarioId: 10,
            setorId: 2,
            prioridadeId: 3,
            criadoEmUtc: new DateTime(2026, 04, 30, 12, 0, 0, DateTimeKind.Utc));

        Assert.Equal(StatusChamado.Aberto, chamado.Status);
        var historicoInicial = Assert.Single(chamado.Historicos);
        Assert.Equal(StatusChamado.Aberto, historicoInicial.Status);
    }

    [Fact]
    public void Iniciar_ChamadoFinalizado_DeveLancarExcecao()
    {
        var chamado = CriarChamadoBase();
        chamado.IniciarAtendimento(20, new DateTime(2026, 04, 30, 12, 5, 0, DateTimeKind.Utc));
        chamado.Finalizar(20, "Acesso restabelecido.", new DateTime(2026, 04, 30, 12, 30, 0, DateTimeKind.Utc));

        var action = () => chamado.IniciarAtendimento(21, new DateTime(2026, 04, 30, 12, 40, 0, DateTimeKind.Utc));

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Somente chamados com status Aberto podem ser iniciados.", exception.Message);
    }

    [Fact]
    public void Finalizar_SemAtendimentoIniciado_DeveLancarExcecao()
    {
        var chamado = CriarChamadoBase();

        var action = () => chamado.Finalizar(20, "Ajuste aplicado.", new DateTime(2026, 04, 30, 12, 30, 0, DateTimeKind.Utc));

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Somente chamados em execução podem ser finalizados.", exception.Message);
    }

    [Fact]
    public void Cancelar_ChamadoFinalizado_DeveLancarExcecao()
    {
        var chamado = CriarChamadoBase();
        chamado.IniciarAtendimento(20, new DateTime(2026, 04, 30, 12, 5, 0, DateTimeKind.Utc));
        chamado.Finalizar(20, "Chamado resolvido.", new DateTime(2026, 04, 30, 12, 30, 0, DateTimeKind.Utc));

        var action = () => chamado.Cancelar(10, new DateTime(2026, 04, 30, 12, 40, 0, DateTimeKind.Utc));

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Não é possível cancelar um chamado finalizado.", exception.Message);
    }

    private static Chamado CriarChamadoBase()
    {
        return Chamado.Criar(
            "Erro no sistema",
            "Nao consigo acessar o modulo financeiro.",
            usuarioId: 10,
            setorId: 2,
            prioridadeId: 3,
            criadoEmUtc: new DateTime(2026, 04, 30, 12, 0, 0, DateTimeKind.Utc));
    }
}
