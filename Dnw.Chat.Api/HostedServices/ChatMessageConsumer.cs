using Dnw.Chat.Api.Services;
using Lib.AspNetCore.ServerSentEvents;

namespace Dnw.Chat.Api.HostedServices;

public class ChatMessageConsumer : BackgroundService
{
    private readonly IChatMessageBusInitializer _chatMessageBusInitializer;
    private readonly IChatConsumer _chatConsumer;
    private readonly IServerSentEventsService _sseService;
    private readonly ILogger<ChatMessageConsumer> _logger;

    public ChatMessageConsumer(
        IChatMessageBusInitializer chatMessageBusInitializer, 
        IChatConsumer chatConsumer,
        IServerSentEventsService sseService,
        ILogger<ChatMessageConsumer> logger)
    {
        _chatMessageBusInitializer = chatMessageBusInitializer;
        _chatConsumer = chatConsumer;
        _sseService = sseService;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ChatMessageConsumer.ExecuteAsync");

        await _chatMessageBusInitializer.Initialize();
        
        await _chatConsumer.Start(msg =>
        {
            _logger.LogInformation("Message received: {message}", msg);
            
            var clients = _sseService.GetClients();
            foreach (var client in clients)
            {
                client.SendEventAsync(msg, cancellationToken).ConfigureAwait(false);
            }
        });
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _chatConsumer.Stop();
        await base.StopAsync(cancellationToken);
    }
}