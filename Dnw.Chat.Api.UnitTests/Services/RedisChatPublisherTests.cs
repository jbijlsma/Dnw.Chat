using Dnw.Chat.Api.Services;
using NSubstitute;
using StackExchange.Redis;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.Services;

public class RedisChatPublisherTests
{
    [Fact]
    public async Task Publish()
    {
        // Given
        const string message = "aMessage";
        
        var mux = Substitute.For<IConnectionMultiplexer>();
        var db = Substitute.For<IDatabase>();
        mux.GetDatabase().Returns(db);
        
        var publisher = new RedisChatPublisher(mux);

        // When
        await publisher.Publish(message);

        // Then
        await db.Received(1).PublishAsync(Arg.Any<RedisChannel>(), message);
    }
}