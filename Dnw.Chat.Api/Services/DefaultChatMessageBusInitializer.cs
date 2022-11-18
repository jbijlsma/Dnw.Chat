namespace Dnw.Chat.Api.Services;

public class DefaultChatMessageBusInitializer : IChatMessageBusInitializer
{
    public Task Initialize()
    {
        return Task.CompletedTask;
    }
}