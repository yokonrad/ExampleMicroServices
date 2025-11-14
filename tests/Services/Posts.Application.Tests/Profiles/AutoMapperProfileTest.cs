using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Posts.Application.Profiles;

namespace Posts.Application.Tests.Profiles;

public class AutoMapperProfileTest
{
    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var mapperConfiguration = new MapperConfiguration(x => x.AddProfile<AutoMapperProfile>(), new NullLoggerFactory());

        // Act
        var act = mapperConfiguration.AssertConfigurationIsValid;

        // Assert
        act.Should().NotThrow<AutoMapperConfigurationException>();
    }
}