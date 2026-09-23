using IdeaForge.Application.Interfaces;
using IdeaForge.Infrastructure.Persistence;
using IdeaForge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdeaForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(PostgresConnectionString.Resolve(configuration)));

        services.AddScoped<IIdeaRepository, IdeaRepository>();

        return services;
    }
}
