using Dnw.Chat.Api.ServiceExtensions;
using Dnw.Chat.Api.Services;
using Dnw.Chat.Api.Services.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.ServiceExtensionTests;

public class KafkaServiceExtensionsTests
{
    [Fact]
    public void AddKafka()
    {
        // Given
        var serviceCollection = new ServiceCollection();

        // When
        serviceCollection.AddKafka("someBootstrapServers");

        // Then
        var expectedRegistrations = new[]
        {
            typeof(IChatMessageBusInitializer),
            typeof(IKafkaProducerFactory),
            typeof(IChatPublisher),
            typeof(IKafkaAdminClientBuilderFactory),
            typeof(IKafkaConsumerFactory),
            typeof(IChatConsumer)
        };

        Assert.Equal(expectedRegistrations.Length, serviceCollection.Count);
        foreach (var expectedRegistration in expectedRegistrations)
        {
            Assert.Contains(serviceCollection, r => r.ServiceType == expectedRegistration);
        }
    }
}