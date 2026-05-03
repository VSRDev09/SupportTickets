using Microsoft.Extensions.DependencyInjection;
using SupportTickets.Application.Interfaces;
using SupportTickets.Application.Services;

namespace SupportTickets.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IChamadoService, ChamadoService>();
        services.AddScoped<ISetorService, SetorService>();
        services.AddScoped<IPrioridadeService, PrioridadeService>();

        return services;
    }
}
