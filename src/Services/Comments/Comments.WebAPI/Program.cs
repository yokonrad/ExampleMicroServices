using Comments.Core.Extensions;
using Comments.WebAPI.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Comments.WebAPI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCommentsCoreSupport(x =>
        {
            if (builder.Environment.IsDevelopment()) x.UseInMemoryDatabase("DbConnection");
            else x.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
        });
        builder.AddCommentsWebAPISupport();

        var app = builder.Build();

        app.Services.AddCommentsCoreSupport();
        app.AddCommentsWebAPISupport();

        await app.RunAsync();
    }
}