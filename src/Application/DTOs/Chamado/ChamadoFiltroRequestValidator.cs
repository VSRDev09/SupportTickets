using FluentValidation;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class ChamadoFiltroRequestValidator : AbstractValidator<ChamadoFiltroRequest>
{
    private static readonly string[] OrdenacoesPermitidas =
    [
        "criadoEm_asc",
        "criadoEm_desc",
        "prioridade_asc",
        "prioridade_desc",
        "status_asc",
        "status_desc"
    ];

    public ChamadoFiltroRequestValidator()
    {
        RuleFor(x => x.SetorId)
            .GreaterThan(0).When(x => x.SetorId.HasValue)
            .WithMessage("O setor informado no filtro é inválido.");

        RuleFor(x => x.PrioridadeId)
            .GreaterThan(0).When(x => x.PrioridadeId.HasValue)
            .WithMessage("A prioridade informada no filtro é inválida.");

        RuleFor(x => x.OrdenarPor)
            .Must(valor => OrdenacoesPermitidas.Contains(valor))
            .WithMessage("A ordenação informada é inválida.");
    }
}
