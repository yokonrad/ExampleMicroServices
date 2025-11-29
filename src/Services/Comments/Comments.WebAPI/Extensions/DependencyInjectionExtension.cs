using Core.WebAPI.Extensions;

namespace Comments.WebAPI.Extensions;

public static class DependencyInjectionExtension
{
    public static WebApplicationBuilder AddCommentsWebAPISupport(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddHealthChecksSupport();
        webApplicationBuilder.AddFastEndpointsSupport("Comments API");

        return webApplicationBuilder;
    }

    public static WebApplication AddCommentsWebAPISupport(this WebApplication webApplication)
    {
        webApplication.AddHealthChecksSupport();
        webApplication.AddFastEndpointsSupport();

        return webApplication;
    }
}