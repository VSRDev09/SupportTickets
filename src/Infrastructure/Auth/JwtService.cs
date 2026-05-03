using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;

namespace SupportTickets.Infrastructure.Auth;

public sealed class JwtService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(Usuario usuario)
    {   
        //Aqui vou criptografar a chave secreta do .env
        //Vou assinar essa chave com a assinatura para a geração do token 
        // e determinar a validade (2 minutos segundo o .env)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            //Aqui estou criando uma lista de informações sobre o usuário para adicionar ao Token
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpirationUtc()
    {
        return DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);
    }
}
