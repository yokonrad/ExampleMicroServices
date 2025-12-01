using Comments.WebAPI.Extensions;
using Core.Application.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace Comments.WebAPI.Tests.Extensions;

public class DependencyInjectionExtensionTest
{
    [Test]
    public void AddCommentsWebAPISupport_WebApplicationBuilder_Should_Be_Valid()
    {
        // Act
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.AddCommentsWebAPISupport();

        // Act
        var act = webApplicationBuilder.Build();

        // Assert
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }

    [Test]
    public void AddCommentsWebAPISupport_WebApplication_Should_Be_Valid()
    {
        // Arrange
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.Services.AddMediatRSupport();
        webApplicationBuilder.AddCommentsWebAPISupport();

        // Act
        var act = webApplicationBuilder.Build();
        act.AddCommentsWebAPISupport();

        // Arrange
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }
}