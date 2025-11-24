using Core.Application.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Posts.WebAPI.Extensions;

namespace Posts.WebAPI.Tests.Extensions;

public class DependencyInjectionExtensionTest
{
    [Test]
    public void AddPostsWebAPISupport_WebApplicationBuilder_Should_Be_Valid()
    {
        // Act
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.AddPostsWebAPISupport();

        // Act
        var act = webApplicationBuilder.Build();

        // Assert
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }

    [Test]
    public void AddPostsWebAPISupport_WebApplication_Should_Be_Valid()
    {
        // Arrange
        var webApplicationBuilder = WebApplication.CreateBuilder();
        webApplicationBuilder.Services.AddMediatRSupport();
        webApplicationBuilder.AddPostsWebAPISupport();

        // Act
        var act = webApplicationBuilder.Build();
        act.AddPostsWebAPISupport();

        // Arrange
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
    }
}