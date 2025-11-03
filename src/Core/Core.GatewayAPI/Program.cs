using Core.GatewayAPI.Extensions;
using Core.WebAPI.Extensions;

namespace Core.GatewayAPI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddHealthChecksSupport();
        builder.AddOcelotSupport();

        var app = builder.Build();

        app.AddHealthChecksSupport();
        app.AddOcelotSupport();

        await app.RunAsync();
    }
}