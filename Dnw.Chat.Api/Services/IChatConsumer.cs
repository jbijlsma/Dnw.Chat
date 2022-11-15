namespace Dnw.Chat.Api.Services;

public interface IChatConsumer
{
    public Task Start(Action<string> onMessage);
}