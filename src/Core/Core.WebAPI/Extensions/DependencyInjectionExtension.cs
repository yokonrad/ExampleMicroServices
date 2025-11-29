using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.WebAPI.Extensions;

public static class DependencyInjectionExtension
{
    public static WebApplicationBuilder AddHealthChecksSupport(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddEndpointsApiExplorer();
        webApplicationBuilder.Services.AddHealthChecks();

        return webApplicationBuilder;
    }

    public static WebApplication AddHealthChecksSupport(this WebApplication webApplication)
    {
        webApplication.UseRouting();
        webApplication.MapHealthChecks("/health").RequireHost("localhost").WithDisplayName("HealthChecks").AllowAnonymous();
        webApplication.UseEndpoints(_ => { });

        return webApplication;
    }

    public static WebApplicationBuilder AddFastEndpointsSupport(this WebApplicationBuilder webApplicationBuilder, string title = "API", string version = "v1")
    {
        webApplicationBuilder.Services.AddFastEndpoints();
        webApplicationBuilder.Services.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.Title = title;
                s.Version = version;
                s.MarkNonNullablePropsAsRequired();
            };

            o.SerializerSettings = s =>
            {
                s.Converters.Add(new JsonStringEnumConverter());
                s.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            };

            o.EnableJWTBearerAuth = false;
            o.RemoveEmptyRequestSchema = true;
            o.ShortSchemaNames = true;
        });

        return webApplicationBuilder;
    }

    public static WebApplication AddFastEndpointsSupport(this WebApplication webApplication, string version = "v1")
    {
        webApplication.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = $"api/{version}";
            c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        webApplication.UseSwaggerGen();

        return webApplication;
    }
}