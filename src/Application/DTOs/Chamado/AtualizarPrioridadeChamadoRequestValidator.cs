using FluentValidation;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class AtualizarPrioridadeChamadoRequestValidator : AbstractValidator<AtualizarPrioridadeChamadoRequest>
{
    public AtualizarPrioridadeChamadoRequestValidator()
    {
        RuleFor(x => x.PrioridadeId)
            .GreaterThan(0).WithMessage("A nova prioridade do chamado é obrigatória.");
    }
}
