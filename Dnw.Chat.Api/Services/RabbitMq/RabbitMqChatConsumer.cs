using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dnw.Chat.Api.Services.RabbitMq;

public class RabbitMqChatConsumer : IChatConsumer, IDisposable
{
    private readonly ILogger<RabbitMqChatConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqChatConsumer(IConnectionFactory connectionFactory, ILogger<RabbitMqChatConsumer> logger)
    {
        _logger = logger;
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task Start(Action<string> onMessage)
    {
        _channel.ExchangeDeclare(exchange: RabbitMqExchanges.Chats, type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName,
            exchange:  RabbitMqExchanges.Chats,
            routingKey: "");
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var msg = Encoding.UTF8.GetString(body);
            _logger.LogInformation("{MachineName} received {message}", Environment.MachineName, msg);
            onMessage(msg);
        };
        
        _channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
    }

    public Task Stop()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _connection.Dispose();
        _channel.Dispose();
    }
}