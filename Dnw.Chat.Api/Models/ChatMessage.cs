using Newtonsoft.Json;

namespace Dnw.Chat.Api.Models;

public class ChatMessage
{
    [JsonProperty] public string Uuid { get; init; } = string.Empty;
    [JsonProperty] public string Message { get; init; } = string.Empty;
}