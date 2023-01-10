using Dnw.Chat.Api.HostedServices;
using Dnw.Chat.Api.ServiceExtensions;
using Dnw.Chat.Api.Services;
using Lib.AspNetCore.ServerSentEvents;

namespace Dnw.Chat.Api;

public class Program {

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Add services to the container.
        if (!Enum.TryParse<PubSubType>(Environment.GetEnvironmentVariable("PUB_SUB_TYPE"), out var pubSubType))
            pubSubType = PubSubType.RabbitMq;

        if (pubSubType == PubSubType.Redis)
        {
            var connectionString = builder.Configuration.GetValue<string>("RedisConnectionString");
            builder.Services.AddRedis(connectionString!);
        }
        else if (pubSubType == PubSubType.Kafka)
        {
            var bootstrapServers = builder.Configuration.GetValue<string>("KafkaBootstrapServers");
            builder.Services.AddKafka(bootstrapServers!);
        }
        else
        {
            var hostName = builder.Configuration.GetValue<string>("RabbitMqHostName");
            builder.Services.AddRabbitMq(hostName!);
        }

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddServerSentEvents();

        builder.Services.AddHostedService<ChatMessageConsumer>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        // the connection for server events
        app.MapServerSentEvents("/chat-updates");

        app.Run();
    }
}
