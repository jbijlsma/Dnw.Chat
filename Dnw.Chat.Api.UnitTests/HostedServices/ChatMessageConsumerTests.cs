using Dnw.Chat.Api.HostedServices;
using Dnw.Chat.Api.Services;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.HostedServices;

public class ChatMessageConsumerTests
{
    [Fact]
    public async Task StartAsync()
    {
        // Given
        const string msg = "aMsg";

        var chatMessageBusInitializer = Substitute.For<IChatMessageBusInitializer>();
        
        var chatConsumer = Substitute.For<IChatConsumer>();
        chatConsumer
            .When(x => x.Start(Arg.Any<Action<string>>()))
            .Do(y =>
            {
                var onMessage = y.Arg<Action<string>>();
                onMessage(msg);
            });
        
        var sseService = Substitute.For<IServerSentEventsService>();
        var sseClients = new[] { Substitute.For<IServerSentEventsClient>() };
        sseService.GetClients().Returns(sseClients);
        
        var logger = Substitute.For<ILogger<ChatMessageConsumer>>();
        
        var hostedService = new ChatMessageConsumer(chatMessageBusInitializer, chatConsumer, sseService, logger);
        
        // When
        await hostedService.StartAsync(CancellationToken.None);

        // Then
        await chatMessageBusInitializer.Received(1).Initialize();
        
        foreach (var client in sseClients)
        {
            await client.Received(1).SendEventAsync(msg, Arg.Any<CancellationToken>());
        }
    }
}