namespace SupportTickets.Application.DTOs.Chamado;

public sealed class CriarChamadoRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int SetorId { get; set; }
    public int PrioridadeId { get; set; }
}
