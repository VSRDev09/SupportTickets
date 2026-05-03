using FluentValidation;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Application.DTOs.Auth;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do usuário deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail do usuário é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado é inválido.")
            .MaximumLength(150).WithMessage("O e-mail do usuário deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha do usuário é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.")
            .MaximumLength(100).WithMessage("A senha deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Perfil)
            .Must(perfil => perfil is PerfilUsuario.Atendente or PerfilUsuario.Usuario)
            .WithMessage("O perfil permitido para cadastro via API é Atendente ou Usuário.");

        RuleFor(x => x.SetorId)
            .GreaterThan(0).WithMessage("O setor do usuário é obrigatório.");
    }
}
