using Demo.Webhooks;
using Messaging.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация Messaging Hosting
builder.Services.AddMessagingHosting();

// Регистрация обработчика webhook'ов
builder.Services.AddSingleton<IWebhookUpdateHandler, WebhookUpdateHandler>();

var app = builder.Build();

// Маппинг endpoint'ов для webhook'ов
app.MapMessagingWebhooks();

app.MapGet("/", () => "Demo.Webhooks is running");

app.Run();

// Expose Program class for integration tests
public partial class Program { }
