using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using Testcontainers.MsSql;

namespace Posts.WebAPI.Tests;

public class ProgramTest
{
    [Test]
    public async Task Should_Be_Valid_When_Environment_Development()
    {
        // Arrange
        await using var webApplicationFactory = new WebApplicationFactory<Program>();

        var client = webApplicationFactory.CreateClient();

        // Act
        var httpResponseMessage = await client.GetAsync("/health");

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Should_Be_Valid_When_Environment_Production()
    {
        // Arrange
        await using var msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await msSqlContainer.StartAsync();

        await using var webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(x =>
        {
            x.UseEnvironment("Production");
            x.UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection([new("ConnectionStrings:DbConnection", msSqlContainer.GetConnectionString())]).Build());
        });

        var client = webApplicationFactory.CreateClient();

        // Act
        var httpResponseMessage = await client.GetAsync("/health");

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}