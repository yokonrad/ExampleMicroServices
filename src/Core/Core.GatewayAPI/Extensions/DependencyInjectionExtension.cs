using MMLib.Ocelot.Provider.AppConfiguration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Core.GatewayAPI.Extensions;

public static class DependencyInjectionExtension
{
    private const string OcelotConfigurationPath = "OcelotConfiguration";

    public static WebApplicationBuilder AddOcelotSupport(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration
            .SetBasePath(webApplicationBuilder.Environment.ContentRootPath)
            .AddJsonFile($"{OcelotConfigurationPath}/ocelot.global.json", false, true)
            .AddJsonFile($"{OcelotConfigurationPath}/ocelot.routes.json", false, true)
            .AddJsonFile($"{OcelotConfigurationPath}/ocelot.swagger.json", false, true)
            .AddEnvironmentVariables();

        webApplicationBuilder.Services.AddSwaggerForOcelot(webApplicationBuilder.Configuration);
        webApplicationBuilder.Services.AddOcelot(webApplicationBuilder.Configuration).AddAppConfiguration();

        return webApplicationBuilder;
    }

    public static WebApplication AddOcelotSupport(this WebApplication webApplication)
    {
        webApplication.UseSwaggerForOcelotUI();
        webApplication.UseOcelot().GetAwaiter().GetResult();

        return webApplication;
    }
}