using Dnw.Chat.HostedServices;
using Dnw.Chat.ServiceExtensions;
using Dnw.Chat.Services;
using Lib.AspNetCore.ServerSentEvents;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
if (!Enum.TryParse<PubSubType>(Environment.GetEnvironmentVariable("PUB_SUB_TYPE"), out var pubSubType))
    pubSubType = PubSubType.RabbitMq;

if (pubSubType == PubSubType.Redis)
{
    builder.Services.AddRedis(builder.Configuration);
}
else
{
    builder.Services.AddRabbitMq(builder.Configuration);
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
