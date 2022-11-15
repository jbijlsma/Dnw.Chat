namespace Dnw.Chat.Services;

public interface IChatConsumer
{
    public Task Start(Action<string> onMessage);
}