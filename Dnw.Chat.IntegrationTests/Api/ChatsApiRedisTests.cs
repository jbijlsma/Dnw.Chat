using System.Net;
using System.Net.Http.Json;
using Dnw.Chat.Api;
using Dnw.Chat.Api.Models;
using Dnw.Chat.Api.Services;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Xunit;

namespace Dnw.Chat.IntegrationTests.Api;

public class ChatsApiRedisTests
{
    private const string PostUrl = "api/chats";
    private const string SseStreamUrl = "/chat-updates";
    
    [Fact]
    public async Task AddChat_Redis()
    {
        // Given
        var chatMessage = new ChatMessage { Uuid = "uuid", Message = "aMessage" };
        
        var redisContainer = new TestcontainersBuilder<RedisTestcontainer>()
            .WithDatabase(new RedisTestcontainerConfiguration("redis:latest"))
            .Build();
        
        await redisContainer.StartAsync().ConfigureAwait(false);

        Environment.SetEnvironmentVariable("PUB_SUB_TYPE", PubSubType.Redis.ToString());
        
        var factory = new ApiFactoryRedis(redisContainer.ConnectionString);

        var client = factory.CreateClient();

        // Observe the SSE stream
        string? actualSseMessage = null;
#pragma warning disable CS4014
        Task.Run(async () =>
#pragma warning restore CS4014
        {
            using var streamReader = new StreamReader(await client.GetStreamAsync(SseStreamUrl));
            while (!streamReader.EndOfStream)
            {
                actualSseMessage = await streamReader.ReadLineAsync();
                if (actualSseMessage != null) return;
            }
        });

        // When
        var actual = await client.PostAsJsonAsync(PostUrl, chatMessage);
        
        // Then
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        
        var expectedSseMessage = $"data: {chatMessage.Uuid} said {chatMessage.Message}";
        Assert.Equal(expectedSseMessage, actualSseMessage);
    }
    
    private class ApiFactoryRedis : WebApplicationFactory<Program>
    {
        private readonly string _redisConnectionString;

        public ApiFactoryRedis(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IConnectionMultiplexer));
                services.AddSingleton<IConnectionMultiplexer>(
                    ConnectionMultiplexer.Connect(_redisConnectionString));
            });
        }
    }
}