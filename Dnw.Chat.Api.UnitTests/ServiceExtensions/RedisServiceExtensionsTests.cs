using Dnw.Chat.Api.ServiceExtensions;
using Dnw.Chat.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.ServiceExtensions;

public class RedisMqServiceExtensionsTests
{
    [Fact]
    public void AddRedis()
    {
        // Given
        var serviceCollection = new ServiceCollection();

        // When
        serviceCollection.AddRedis("someConnectionString");

        // Then
        var expectedRegistrations = new[]
        {
            typeof(IChatMessageBusInitializer),
            typeof(IConnectionMultiplexer),
            typeof(IChatPublisher),
            typeof(IChatConsumer)
        };

        Assert.Equal(expectedRegistrations.Length, serviceCollection.Count);
        foreach (var expectedRegistration in expectedRegistrations)
        {
            Assert.Contains(serviceCollection, r => r.ServiceType == expectedRegistration);
        }
    }
}