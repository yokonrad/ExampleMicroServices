using Core.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Posts.Domain.Interfaces;
using Posts.Infrastructure.Data;
using Posts.Infrastructure.Repositories;

namespace Posts.Infrastructure.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddPostsInfrastructureSupport(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> actionDbContextOptionsBuilder)
    {
        serviceCollection.AddEntityFrameworkCoreSupport<PostDbContext>(actionDbContextOptionsBuilder);
        serviceCollection.AddScoped<IPostRepository, PostRepository>();

        return serviceCollection;
    }

    public static IServiceProvider AddPostsInfrastructureSupport(this IServiceProvider serviceProvider)
    {
        serviceProvider.AddEntityFrameworkCoreSupport<PostDbContext>();

        return serviceProvider;
    }
}