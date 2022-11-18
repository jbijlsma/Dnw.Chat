using Confluent.Kafka.Admin;

namespace Dnw.Chat.Api.Services.Kafka;

public class KafkaChatMessageBusInitializer : IChatMessageBusInitializer
{
    private readonly IKafkaAdminClientBuilderFactory _adminClientBuilderFactory;
    private readonly ILogger<KafkaChatMessageBusInitializer> _logger;

    public KafkaChatMessageBusInitializer(
        IKafkaAdminClientBuilderFactory adminClientBuilderFactory,
        ILogger<KafkaChatMessageBusInitializer> logger)
    {
        _adminClientBuilderFactory = adminClientBuilderFactory;
        _logger = logger;
    }
    
    public async Task Initialize()
    {
        await CreateTopic();
    }
    
    private async Task CreateTopic()
    {
        var topic = KafkaTopics.ChatMessages;

        try
        {

            var builder = _adminClientBuilderFactory.Create();
            using var adminClient = builder.Build();
            await adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = topic,
                    ReplicationFactor = 1,
                    NumPartitions = 1
                }
            });
            
            _logger.LogInformation("Kafka topic created: {topic}", topic);
        }
        catch (CreateTopicsException ex)
        {
            if (ex.Message.Contains($"Topic '{topic}' already exists", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Topic already exists: {topic}", topic);   
            }
            else
            {
                throw;
            }
        }
    }
}