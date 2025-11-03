using Core.WebAPI.Extensions;

namespace Posts.WebAPI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddHealthChecksSupport();
        builder.AddFastEndpointsSupport("Posts API");

        var app = builder.Build();

        app.AddHealthChecksSupport();
        app.AddFastEndpointsSupport();

        await app.RunAsync();
    }
}