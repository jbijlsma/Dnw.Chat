using Confluent.Kafka;

namespace Dnw.Chat.Api.Services.Kafka;

public interface IKafkaAdminClientBuilderFactory
{
    AdminClientBuilder Create();
}

public class KafkaAdminClientBuilderFactory : IKafkaAdminClientBuilderFactory
{
    private readonly string _bootstrapServers;

    public KafkaAdminClientBuilderFactory(string bootstrapServers)
    {
        _bootstrapServers = bootstrapServers;
    }
    
    public AdminClientBuilder Create()
    {
        return new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _bootstrapServers
        });
    }
}