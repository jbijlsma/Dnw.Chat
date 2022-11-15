using Dnw.Chat.Models;
using Dnw.Chat.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dnw.Chat.Controllers;

[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IChatPublisher _chatPublisher;
    private readonly ILogger<ChatsController> _logger;

    public ChatsController(IChatPublisher chatPublisher, ILogger<ChatsController> logger)
    {
        _chatPublisher = chatPublisher;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody]ChatMessage chatMsg)
    {
        _logger.LogWarning("{MachineName} received {Message}", Environment.MachineName, chatMsg.Message);
        await _chatPublisher.Publish($"{chatMsg.Uuid} said {chatMsg.Message}"); 
        return Ok(Environment.MachineName);
    }
}