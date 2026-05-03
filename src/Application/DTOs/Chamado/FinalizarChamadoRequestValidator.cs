using FluentValidation;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class FinalizarChamadoRequestValidator : AbstractValidator<FinalizarChamadoRequest>
{
    public FinalizarChamadoRequestValidator()
    {
        RuleFor(x => x.Solucao)
            .NotEmpty().WithMessage("A solução do chamado é obrigatória.")
            .MaximumLength(2000).WithMessage("A solução do chamado deve ter no máximo 2000 caracteres.");
    }
}
