using Core.WebAPI.Extensions;

namespace Comments.WebAPI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddHealthChecksSupport();
        builder.AddFastEndpointsSupport("Comments API");

        var app = builder.Build();

        app.AddHealthChecksSupport();
        app.AddFastEndpointsSupport();

        await app.RunAsync();
    }
}