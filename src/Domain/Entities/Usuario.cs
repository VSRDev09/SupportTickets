using System.Net.Mail;
using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Domain.Entities;

public class Usuario
{
    private Usuario()
    {
    }

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public PerfilUsuario Perfil { get; private set; }
    public int SetorId { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public bool Ativo { get; private set; }

    public Setor Setor { get; private set; } = null!;

    public static Usuario Criar(
        string nome,
        string email,
        string senhaHash,
        PerfilUsuario perfil,
        int setorId,
        DateTime criadoEmUtc)
    {
        Validar(nome, email, senhaHash, setorId);

        return new Usuario
        {
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            SenhaHash = senhaHash,
            Perfil = perfil,
            SetorId = setorId,
            CriadoEm = criadoEmUtc,
            Ativo = true
        };
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public void Ativar()
    {
        Ativo = true;
    }

    private static void Validar(string nome, string email, string senhaHash, int setorId)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do usuário é obrigatório.");
        }

        if (nome.Trim().Length > 150)
        {
            throw new DomainException("O nome do usuário deve ter no máximo 150 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("O e-mail do usuário é obrigatório.");
        }

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new DomainException("O e-mail informado é inválido.");
        }

        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new DomainException("O hash da senha do usuário é obrigatório.");
        }

        if (setorId <= 0)
        {
            throw new DomainException("O setor do usuário é obrigatório.");
        }
    }
}
