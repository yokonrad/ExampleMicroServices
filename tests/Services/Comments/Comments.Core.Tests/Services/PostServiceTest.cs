using AutoBogus;
using Comments.Core.Dtos;
using Comments.Core.Extensions;
using Comments.Core.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Net;

namespace Comments.Core.Tests.Services;

public class PostServiceTest
{
    private ServiceProvider serviceProvider;
    private Mock<IHttpClientFactory> mockHttpClientFactory;
    private Mock<IConfiguration> mockConfiguration;

    [SetUp]
    public async Task SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCommentsCoreSupport(x => x.UseInMemoryDatabase("DbConnection"));
        serviceCollection.AddLogging(x => x.AddProvider(NullLoggerProvider.Instance));

        serviceProvider = serviceCollection.BuildServiceProvider();
        mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockConfiguration = new Mock<IConfiguration>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await serviceProvider.DisposeAsync();
    }

    [Test]
    public async Task Should_Be_Invalid()
    {
        // Arrange
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.When($"http://localhost:5001/api/v1/posts/{It.IsAny<Guid>()}").Respond(HttpStatusCode.NotFound);

        mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockHttpMessageHandler));
        mockConfiguration.SetupGet(x => x["Services:Posts"]).Returns("http://localhost:5001");

        var postService = new PostService(mockHttpClientFactory.Object, mockConfiguration.Object);

        // Act
        var act = await postService.GetByGuidAsync(It.IsAny<Guid>());

        // Assert
        act.Should().BeNull();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.When($"http://localhost:5001/api/v1/posts/{postDto.Guid}").Respond("application/json", JsonConvert.SerializeObject(postDto));

        mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockHttpMessageHandler));
        mockConfiguration.SetupGet(x => x["Services:Posts"]).Returns("http://localhost:5001");

        var postService = new PostService(mockHttpClientFactory.Object, mockConfiguration.Object);

        // Act
        var act = await postService.GetByGuidAsync(postDto.Guid);

        // Assert
        act.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }
}