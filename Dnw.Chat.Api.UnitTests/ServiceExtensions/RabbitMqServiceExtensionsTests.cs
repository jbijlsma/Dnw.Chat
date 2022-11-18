using Dnw.Chat.Api.ServiceExtensions;
using Dnw.Chat.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.ServiceExtensions;

public class RabbitMqServiceExtensionsTests
{
    [Fact]
    public void AddRabbitMq()
    {
        // Given
        var serviceCollection = new ServiceCollection();

        // When
        serviceCollection.AddRabbitMq("SomeHostName");

        // Then
        var expectedRegistrations = new[]
        {
            typeof(IChatMessageBusInitializer),
            typeof(IConnectionFactory), 
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