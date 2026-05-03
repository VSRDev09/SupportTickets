using FluentValidation;

namespace SupportTickets.Application.DTOs.Chamado;

public sealed class CriarChamadoRequestValidator : AbstractValidator<CriarChamadoRequest>
{
    public CriarChamadoRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O título do chamado é obrigatório.")
            .MaximumLength(200).WithMessage("O título do chamado deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do chamado é obrigatória.");

        RuleFor(x => x.SetorId)
            .GreaterThan(0).WithMessage("O setor do chamado é obrigatório.");

        RuleFor(x => x.PrioridadeId)
            .GreaterThan(0).WithMessage("A prioridade do chamado é obrigatória.");
    }
}
