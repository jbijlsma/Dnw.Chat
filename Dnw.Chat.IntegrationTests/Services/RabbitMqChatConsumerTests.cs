using System.Text;
using Dnw.Chat.Api.Services.RabbitMq;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RabbitMQ.Client;
using Xunit;

namespace Dnw.Chat.IntegrationTests.Services;

public class RabbitMqChatConsumerTests
{
    [Fact]
    public async Task Start()
    {
        // Given
        const string message = "aMessage";
        const ushort rabbitMqPort = 5672;
        
        var rabbitMqContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithImage("rabbitmq:3.11-management")
            .WithPortBinding(rabbitMqPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(rabbitMqPort))
            .Build();

        await rabbitMqContainer.StartAsync()
            .ConfigureAwait(false);

        var connectionFactory = new ConnectionFactory { Port = rabbitMqContainer.GetMappedPublicPort(rabbitMqPort) };
        var logger = Substitute.For<ILogger<RabbitMqChatConsumer>>();

        var onMessage = Substitute.For<Action<string>>();
        
        var consumer = new RabbitMqChatConsumer(connectionFactory, logger);
        await consumer.Start(onMessage);
        
        // When
        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: RabbitMqExchanges.Chats, type: ExchangeType.Fanout);
        channel.BasicPublish(exchange: RabbitMqExchanges.Chats,
            routingKey: "",
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(message));

        await Task.Delay(500);
        
        consumer.Dispose();

        // Then
        onMessage.Received(1).Invoke(message);
    }
}