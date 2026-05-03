using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Domain.Entities;

public class Setor
{
    private Setor()
    {
    }

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public static Setor Criar(string nome, string? descricao)
    {
        Validar(nome, descricao);

        return new Setor
        {
            Nome = nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim()
        };
    }

    public void Atualizar(string nome, string? descricao)
    {
        Validar(nome, descricao);

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    private static void Validar(string nome, string? descricao)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do setor é obrigatório.");
        }

        if (nome.Trim().Length > 100)
        {
            throw new DomainException("O nome do setor deve ter no máximo 100 caracteres.");
        }

        if (!string.IsNullOrWhiteSpace(descricao) && descricao.Trim().Length > 500)
        {
            throw new DomainException("A descrição do setor deve ter no máximo 500 caracteres.");
        }
    }
}
