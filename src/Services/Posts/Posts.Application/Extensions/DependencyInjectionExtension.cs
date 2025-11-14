using Core.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Posts.Application.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddPostsApplicationSupport(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
        serviceCollection.AddFluentValidationSupport();
        serviceCollection.AddMediatRSupport();

        return serviceCollection;
    }
}