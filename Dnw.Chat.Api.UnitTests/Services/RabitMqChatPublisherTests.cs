using System.Text;
using Dnw.Chat.Api.Services.RabbitMq;
using NSubstitute;
using RabbitMQ.Client;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.Services;

public class RabbitMqChatPublisherTests
{
    [Fact]
    public async Task Publish()
    {
        // Given
        const string message = "aMessage";
        
        var connectionFactory = Substitute.For<IConnectionFactory>();
        var connection = Substitute.For<IConnection>();
        connectionFactory.CreateConnection().Returns(connection);
        var channel = Substitute.For<IModel>();
        connection.CreateModel().Returns(channel);
        
        var publisher = new RabbitMqChatPublisher(connectionFactory);

        // When
        await publisher.Publish(message);

        // Then
        channel.Received(1).ExchangeDeclare(exchange: RabbitMqExchanges.Chats, type: ExchangeType.Fanout);
        channel.Received(1).BasicPublish(
            RabbitMqExchanges.Chats,
            "",
            null,
            Arg.Is<ReadOnlyMemory<byte>>(_ => Encoding.UTF8.GetString(_.ToArray()) == message));
    }
}