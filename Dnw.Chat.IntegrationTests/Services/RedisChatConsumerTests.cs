using Dnw.Chat.Api.Services;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StackExchange.Redis;
using Xunit;

namespace Dnw.Chat.IntegrationTests.Services;

public class RedisChatConsumerTests
{
    [Fact]
    public async Task Start()
    {
        // Given
        const string message = "aMessage";
        
        var redisContainer = new TestcontainersBuilder<RedisTestcontainer>()
            .WithDatabase(new RedisTestcontainerConfiguration())
            .Build();

        await redisContainer.StartAsync()
            .ConfigureAwait(false);

        await using var mux = await ConnectionMultiplexer.ConnectAsync(redisContainer.ConnectionString).ConfigureAwait(false);
       
        var logger = Substitute.For<ILogger<RedisChatConsumer>>();

        var onMessage = Substitute.For<Action<string>>();
        
        var consumer = new RedisChatConsumer(mux, logger);
        await consumer.Start(onMessage);
        
        // When
        await mux.GetDatabase().PublishAsync(
            new RedisChannel(RedisChannels.ChatMessages, RedisChannel.PatternMode.Literal), 
            message);
        
        await Task.Delay(500);

        // Then
        onMessage.Received(1).Invoke(message);
    }
}