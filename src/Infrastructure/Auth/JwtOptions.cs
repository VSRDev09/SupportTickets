namespace SupportTickets.Infrastructure.Auth;

/// <summary>
/// Essa classe aqui vai representar as configurações de autenticação via JWT.
/// Esses valores são carregados da minha .env
/// Estou mantendo na camada de infraestrutura por ser um detalhe técnico de autenticação.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    // - Issuer: identifica quem emitiu o token
    public string Issuer { get; init; } = string.Empty;
    
    // - Audience: define quem pode consumir o token
    public string Audience { get; init; } = string.Empty;

    /// - SecretKey: chave usada para assinar e validar o token 
    public string SecretKey { get; init; } = string.Empty;

    /// - ExpirationMinutes: tempo de validade do token
    public int ExpirationMinutes { get; init; } = 120;
}
