using AutoMapper;
using Comments.Core.Profiles;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Comments.Core.Tests.Profiles;

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