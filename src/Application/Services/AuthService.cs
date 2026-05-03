using SupportTickets.Application.DTOs.Auth;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISetorRepository _setorRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ISetorRepository setorRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _setorRepository = setorRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (usuario is null || !_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
        {
            throw new UnauthorizedException("E-mail ou senha inválidos.");
        }

        if (!usuario.Ativo)
        {
            throw new UnauthorizedException("Este usuário está inativo e não pode acessar o sistema.");
        }

        return new LoginResponse
        {
            Token = _tokenService.GenerateToken(usuario),
            ExpiraEmUtc = _tokenService.GetExpirationUtc(),
            Usuario = MapUser(usuario)
        };
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(email, cancellationToken);

        if (usuarioExistente is not null)
        {
            throw new DomainException("Já existe um usuário cadastrado com este e-mail.");
        }

        if (!await _setorRepository.ExistsAsync(request.SetorId, cancellationToken))
        {
            throw new NotFoundException("O setor informado para o usuário não foi encontrado.");
        }

        var usuario = Usuario.Criar(
            request.Nome,
            email,
            _passwordHasher.Hash(request.Senha),
            request.Perfil,
            request.SetorId,
            DateTime.UtcNow);

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _usuarioRepository.SaveChangesAsync(cancellationToken);

        usuario = await _usuarioRepository.GetByIdAsync(usuario.Id, cancellationToken)
                  ?? usuario;

        return MapUser(usuario);
    }

    private static UserResponse MapUser(Usuario usuario)
    {
        return new UserResponse
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            SetorId = usuario.SetorId,
            SetorNome = usuario.Setor?.Nome,
            Ativo = usuario.Ativo
        };
    }
}
