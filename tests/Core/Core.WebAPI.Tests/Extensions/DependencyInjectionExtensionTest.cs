using Core.Application.Extensions;
using Core.WebAPI.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace Core.WebAPI.Tests.Extensions;

public class DependencyInjectionExtensionTest
{
    [Test]
    public void AddHealthChecksSupport_WebApplicationBuilder_Should_Be_Valid()
    {
        // Arrange
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.AddHealthChecksSupport();

        // Act
        var act = webApplicationBuilder.Build();

        // Assert
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }

    [Test]
    public void AddHealthChecksSupport_WebApplication_Should_Be_Valid()
    {
        // Arrange
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.AddHealthChecksSupport();

        // Act
        var act = webApplicationBuilder.Build();
        act.AddHealthChecksSupport();

        // Arrange
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }

    [Test]
    public void AddFastEndpointsSupport_WebApplicationBuilder_Should_Be_Valid()
    {
        // Arrange
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.AddFastEndpointsSupport();

        // Act
        var act = webApplicationBuilder.Build();

        // Assert
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }

    [Test]
    public void AddFastEndpointsSupport_WebApplication_Should_Be_Valid()
    {
        // Arrange
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.Services.AddMediatRSupport();
        webApplicationBuilder.AddFastEndpointsSupport();

        // Act
        var act = webApplicationBuilder.Build();
        act.AddFastEndpointsSupport();

        // Arrange
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }
}