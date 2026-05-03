using FluentValidation;

namespace SupportTickets.Application.DTOs.Prioridade;

public sealed class PrioridadeRequestValidator : AbstractValidator<PrioridadeRequest>
{
    public PrioridadeRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da prioridade é obrigatório.")
            .MaximumLength(50).WithMessage("O nome da prioridade deve ter no máximo 50 caracteres.");

        RuleFor(x => x.TempoEstimadoHoras)
            .GreaterThan(0).WithMessage("O tempo estimado em horas deve ser maior que zero.");
    }
}
