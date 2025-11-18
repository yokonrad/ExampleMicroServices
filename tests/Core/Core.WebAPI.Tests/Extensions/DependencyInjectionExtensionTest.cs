using Core.Application.Extensions;
using Core.WebAPI.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

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
        var actActionDescriptorCollectionProviderServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IActionDescriptorCollectionProvider).Name && x.ImplementationType?.Name == "DefaultActionDescriptorCollectionProvider");
        var actApiDescriptionGroupCollectionProviderServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IApiDescriptionGroupCollectionProvider).Name);
        var actApiDescriptionProviderServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IApiDescriptionProvider).Name && x.ImplementationType?.Name == "EndpointMetadataApiDescriptionProvider");
        var actHealthCheckServiceServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(HealthCheckService).Name && x.ImplementationType?.Name == "DefaultHealthCheckService");
        var actHostedServiceServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IHostedService).Name && x.ImplementationType?.Name == "HealthCheckPublisherHostedService");

        // Assert
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
        actActionDescriptorCollectionProviderServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actApiDescriptionGroupCollectionProviderServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actApiDescriptionProviderServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actHealthCheckServiceServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actHostedServiceServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
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
        var actCommandHandlerRegistryServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == "CommandHandlerRegistry");
        var actEndpointDataServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == "EndpointData");
        var actHttpContextAccessorServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IHttpContextAccessor).Name && x.ImplementationType?.Name == typeof(HttpContextAccessor).Name);
        var actServiceResolverServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IServiceResolver).Name && x.ImplementationType?.Name == "ServiceResolver");
        var actEndpointFactoryServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IEndpointFactory).Name && x.ImplementationType?.Name == typeof(EndpointFactory).Name);
        var actRequestBinderServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(IRequestBinder<>).Name && x.ImplementationType?.Name == typeof(RequestBinder<>).Name);
        var actEventBusServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(EventBus<>).Name && x.ImplementationType?.Name == typeof(EventBus<>).Name);
        var actConfigServiceDescriptor = webApplicationBuilder.Services.First(x => x.ServiceType.Name == typeof(Config).Name && x.ImplementationType?.Name == typeof(Config).Name);

        // Assert
        act.Should().BeOfType<WebApplication>().And.NotBeNull();
        actCommandHandlerRegistryServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actEndpointDataServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actHttpContextAccessorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actServiceResolverServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actEndpointFactoryServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actRequestBinderServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actEventBusServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actConfigServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
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