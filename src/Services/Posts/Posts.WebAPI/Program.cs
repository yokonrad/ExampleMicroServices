using Microsoft.EntityFrameworkCore;
using Posts.Application.Extensions;
using Posts.Infrastructure.Extensions;
using Posts.WebAPI.Extensions;

namespace Posts.WebAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPostsApplicationSupport();
        builder.Services.AddPostsInfrastructureSupport(x =>
        {
            if (builder.Environment.IsDevelopment()) x.UseInMemoryDatabase("DbConnection");
            else x.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
        });
        builder.AddPostsWebAPISupport();

        var app = builder.Build();

        app.Services.AddPostsInfrastructureSupport();
        app.AddPostsWebAPISupport();

        await app.RunAsync();
    }
}