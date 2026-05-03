using SupportTickets.Domain.Entities;

namespace SupportTickets.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
    DateTime GetExpirationUtc();
}
