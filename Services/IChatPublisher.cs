namespace Dnw.Chat.Services;

public interface IChatPublisher
{
    public Task Publish(string msg);
}