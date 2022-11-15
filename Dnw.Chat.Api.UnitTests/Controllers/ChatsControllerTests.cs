using Dnw.Chat.Api.Controllers;
using Dnw.Chat.Api.Models;
using Dnw.Chat.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Dnw.Chat.Api.UnitTests.Controllers;

public class ChatsControllerTests
{
    [Fact]
    public async Task AddChat()
    {
        // Given
        var chatPublisher = Substitute.For<IChatPublisher>();
        var logger = Substitute.For<ILogger<ChatsController>>();
        
        var controller = new ChatsController(chatPublisher, logger);

        // When
        var actual = await controller.AddChat(new ChatMessage { Uuid = "someUuid", Message = "aMessage" });

        // Then
        Assert.IsType<OkObjectResult>(actual);
    }
}