using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddEntityFrameworkCoreSupport<T>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> actionDbContextOptionsBuilder) where T : DbContext
    {
        serviceCollection.AddDbContext<T>(actionDbContextOptionsBuilder);

        return serviceCollection;
    }

    public static IServiceProvider AddEntityFrameworkCoreSupport<T>(this IServiceProvider serviceProvider) where T : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<T>();

        dbContext.Database.EnsureCreated();

        return serviceProvider;
    }
}