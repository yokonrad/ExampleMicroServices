using Comments.Core.Data;
using Comments.Core.Interfaces;
using Comments.Core.Repositories;
using Comments.Core.Services;
using Core.Application.Extensions;
using Core.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Comments.Core.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddCommentsCoreSupport(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> actionDbContextOptionsBuilder)
    {
        serviceCollection.AddEntityFrameworkCoreSupport<CommentDbContext>(actionDbContextOptionsBuilder);

        serviceCollection.AddHttpClient();
        serviceCollection.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
        serviceCollection.AddFluentValidationSupport();
        serviceCollection.AddMediatRSupport();

        serviceCollection.AddScoped<ICommentRepository, CommentRepository>();
        serviceCollection.AddScoped<IPostService, PostService>();

        return serviceCollection;
    }

    public static IServiceProvider AddCommentsCoreSupport(this IServiceProvider serviceProvider)
    {
        serviceProvider.AddEntityFrameworkCoreSupport<CommentDbContext>();

        return serviceProvider;
    }
}