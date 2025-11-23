using Core.WebAPI.Extensions;

namespace Posts.WebAPI.Extensions;

public static class DependencyInjectionExtension
{
    public static WebApplicationBuilder AddPostsWebAPISupport(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddHealthChecksSupport();
        webApplicationBuilder.AddFastEndpointsSupport("Posts API");

        return webApplicationBuilder;
    }

    public static WebApplication AddPostsWebAPISupport(this WebApplication webApplication)
    {
        webApplication.AddHealthChecksSupport();
        webApplication.AddFastEndpointsSupport();

        return webApplication;
    }
}