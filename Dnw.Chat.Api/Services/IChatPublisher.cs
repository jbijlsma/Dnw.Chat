namespace Dnw.Chat.Api.Services;

public interface IChatPublisher
{
    public Task Publish(string msg);
}