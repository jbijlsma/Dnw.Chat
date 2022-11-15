using System.Net;
using System.Net.Http.Json;
using Dnw.Chat.Api;
using Dnw.Chat.Api.Models;
using Dnw.Chat.Api.Services;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using Xunit;

namespace Dnw.Chat.IntegrationTests.Api;

public class ChatsApiRabbitMqTests
{
    private const string PostUrl = "api/chats";
    private const string SseStreamUrl = "/chat-updates";
    
    [Fact]
    public async Task AddChat_RabbitMq()
    {
        // Given
        var chatMessage = new ChatMessage { Uuid = "uuid", Message = "aMessage" };
        const ushort rabbitMqPort = 5672;
        
        var rabbitMqContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithImage("rabbitmq:3.11-management")
            .WithPortBinding(rabbitMqPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(rabbitMqPort))
            .Build();

        await rabbitMqContainer.StartAsync()
            .ConfigureAwait(false);

        Environment.SetEnvironmentVariable("PUB_SUB_TYPE", PubSubType.RabbitMq.ToString());
        
        var factory = new ApiFactoryRabbitMq(rabbitMqContainer.Hostname, rabbitMqContainer.GetMappedPublicPort(rabbitMqPort));

        var client = factory.CreateClient();

        // Observe the SSE stream on a background thread
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
    
    private class ApiFactoryRabbitMq : WebApplicationFactory<Program>
    {
        private readonly string _hostName;
        private readonly int _port;

        public ApiFactoryRabbitMq(string hostName, int port)
        {
            _hostName = hostName;
            _port = port;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IConnectionFactory));
                services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory { HostName = _hostName, Port = _port });
            });
        }
    }
}