using System.Text;
using RabbitMQ.Client;

namespace Dnw.Chat.Api.Services.RabbitMq;

public class RabbitMqChatPublisher : IChatPublisher
{
    private readonly IConnectionFactory _connectionFactory;

    public RabbitMqChatPublisher(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public Task Publish(string msg)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: RabbitMqExchanges.Chats, type: ExchangeType.Fanout);
        
        var body = Encoding.UTF8.GetBytes(msg);
        channel.BasicPublish(exchange: RabbitMqExchanges.Chats,
            routingKey: "",
            basicProperties: null,
            body: body);
        
        return Task.CompletedTask;
    }
}