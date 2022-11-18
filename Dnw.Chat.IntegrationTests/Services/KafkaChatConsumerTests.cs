using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Dnw.Chat.Api.Services.Kafka;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Dnw.Chat.IntegrationTests.Services;

public class KafkaChatConsumerTests
{
    private readonly ITestOutputHelper _output;

    public KafkaChatConsumerTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    /// <summary>
    /// After playing around with this for some time I find there are quite some issues
    /// and I see no other way then to build in some wait times with Task.Delay()
    /// 
    /// See the code below for details
    /// </summary>
    [Fact]
    public async Task Start()
    {
        // Given
        const string message = "aMessage";
        
        var kafkaContainer = new TestcontainersBuilder<KafkaTestcontainer>()
            .WithKafka(new KafkaTestcontainerConfiguration())
            // ReSharper disable once StringLiteralTypo
            .WithImage("confluentinc/cp-kafka:7.3.0")
            .Build();

        await kafkaContainer.StartAsync().ConfigureAwait(false);
        
        _output.WriteLine("Kafka container started ..");

        // Create the topic
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = kafkaContainer.BootstrapServers
        }).Build();
        await adminClient.CreateTopicsAsync(new[] { 
            new TopicSpecification
            {
                Name = KafkaTopics.ChatMessages, 
                ReplicationFactor = 1, 
                NumPartitions = 1
            }
        });
        
        _output.WriteLine("Topic created ..");

        var consumerFactory = new KafkaConsumerFactory( 
            new ConsumerConfig
            {
                BootstrapServers = kafkaContainer.BootstrapServers,
                GroupId = Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Latest
            });
        
        var logger = Substitute.For<ILogger<KafkaChatConsumer>>();

        var onMessage = Substitute.For<Action<string>>();
        onMessage
            .When(x => x.Invoke(Arg.Any<string>()))
            .Do(y =>
            {
                var msg = y.Arg<string>();
                _output.WriteLine($"Processed message: {msg}");
            });

        var consumer = new KafkaChatConsumer(consumerFactory, logger);
#pragma warning disable CS4014
        Task.Run(() => consumer.Start(onMessage)).ConfigureAwait(false);
#pragma warning restore CS4014
        
        _output.WriteLine("Subscribed on topic ..");

        // Subscribing to a topic seems to take quite some time
        // and there is no way to wait for it
        // 1.5 seconds seems to be enough so far
        await Task.Delay(1500);
        
        _output.WriteLine("Waited 1500ms for subscription to complete ..");
        
        // When
        var producerFactory = new KafkaProducerFactory(new ProducerConfig
        {
            BootstrapServers = kafkaContainer.BootstrapServers
        });
        using var producer = producerFactory.CreateProducer<Null, string>();
        await producer.ProduceAsync(KafkaTopics.ChatMessages, new Message<Null, string> { Value = message });
        
        _output.WriteLine("Produced message. Will wait 200ms for it to the processed ..");
        
        // Picking up a message after it has been produced seems to be quite fast
        // A 200 ms delay seems to be ok
        await Task.Delay(200);
        
        await consumer.Stop();
        
        _output.WriteLine("Consumer loop stopped ..");

        // Then
        onMessage.Received(1).Invoke(message);
    }
}