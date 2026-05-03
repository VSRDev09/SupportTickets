using FluentValidation;

namespace SupportTickets.Application.DTOs.Setor;

public sealed class SetorRequestValidator : AbstractValidator<SetorRequest>
{
    public SetorRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do setor é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do setor deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("A descrição do setor deve ter no máximo 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));
    }
}
