using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Domain.Entities;

public class Prioridade
{
    private Prioridade()
    {
    }

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public int TempoEstimadoHoras { get; private set; }

    public static Prioridade Criar(string nome, int tempoEstimadoHoras)
    {
        Validar(nome, tempoEstimadoHoras);

        return new Prioridade
        {
            Nome = nome.Trim(),
            TempoEstimadoHoras = tempoEstimadoHoras
        };
    }

    public void Atualizar(string nome, int tempoEstimadoHoras)
    {
        Validar(nome, tempoEstimadoHoras);

        Nome = nome.Trim();
        TempoEstimadoHoras = tempoEstimadoHoras;
    }

    private static void Validar(string nome, int tempoEstimadoHoras)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome da prioridade é obrigatório.");
        }

        if (nome.Trim().Length > 50)
        {
            throw new DomainException("O nome da prioridade deve ter no máximo 50 caracteres.");
        }

        if (tempoEstimadoHoras <= 0)
        {
            throw new DomainException("O tempo estimado da prioridade deve ser maior que zero.");
        }
    }
}
