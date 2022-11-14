using Dnw.Chat.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Dnw.Chat.Controllers;

[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly ILogger<ChatsController> _logger;
    private readonly IDatabase _db;

    public ChatsController(IConnectionMultiplexer mux, ILogger<ChatsController> logger)
    {
        _logger = logger;
        _db = mux.GetDatabase();
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody]ChatMessage chatMsg)
    {
        _logger.LogWarning("{MachineName} received {Message}", Environment.MachineName, chatMsg.Message);
        await _db.PublishAsync(new RedisChannel(RedisChannels.ChatMessages, RedisChannel.PatternMode.Literal),$"{chatMsg.Uuid} said {chatMsg.Message}");
        return Ok(Environment.MachineName);
    }
}