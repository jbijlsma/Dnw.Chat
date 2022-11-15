using Dnw.Chat.Api.ServiceExtensions;
using Dnw.Chat.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.ServiceExtensionTests;

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
        var expectedRegistrations = new[] { typeof(IConnectionFactory), typeof(IChatPublisher), typeof(IChatConsumer) };
        foreach (var expectedRegistration in expectedRegistrations)
        {
            Assert.Contains(serviceCollection, r => r.ServiceType == expectedRegistration);
        }
    }
}