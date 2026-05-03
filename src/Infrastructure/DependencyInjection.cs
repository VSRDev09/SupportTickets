using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportTickets.Application.Interfaces;
using SupportTickets.Infrastructure.Auth;
using SupportTickets.Infrastructure.Data;
using SupportTickets.Infrastructure.Repositories;
using Npgsql;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Infrastructure;

public static class DependencyInjection
{
    public class UpperCaseNameTranslator : Npgsql.INpgsqlNameTranslator
    {
        public string TranslateMemberName(string clrName) => clrName.ToUpperInvariant();
        public string TranslateTypeName(string clrName) => clrName;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("A connection string DefaultConnection não foi configurada.");

        // Estou configurando o Enum aqui
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        var translator = new UpperCaseNameTranslator();

        dataSourceBuilder.MapEnum<StatusChamado>("status_chamado", translator);
        dataSourceBuilder.MapEnum<PerfilUsuario>("perfil_usuario", translator);
        var dataSource = dataSourceBuilder.Build();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dataSource));

        services.AddScoped<IChamadoRepository, ChamadoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISetorRepository, SetorRepository>();
        services.AddScoped<IPrioridadeRepository, PrioridadeRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtService>();

        return services;
    }

}
