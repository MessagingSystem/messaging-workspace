using Demo.Webhooks;
using Messaging.Abstractions;
using Messaging.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация Messaging Hosting
builder.Services.AddMessagingHosting();

// Регистрация обработчика webhook'ов
builder.Services.AddSingleton<IWebhookUpdateHandler, WebhookUpdateHandler>();

var app = builder.Build();

// Маппинг endpoint'ов для webhook'ов
app.MapMessagingWebhooks();

app.MapGet("/demo/send-keyboard/{provider}", async (
    string provider,
    string? chatId,
    IChatClient chatClient,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(chatId))
    {
        return Results.BadRequest("chatId query parameter is required.");
    }

    var keyboard = new InlineKeyboard(new[]
    {
        new InlineKeyboardRow(new InlineKeyboardButton[]
        {
            new CallbackButton("OK", "ok"),
            new UrlButton("Docs", new Uri("https://example.com/docs"))
        })
    });

    var request = new SendTextRequest(
        new ChatId(chatId),
        $"Inline keyboard demo ({provider})")
    {
        Keyboard = keyboard
    };

    await chatClient.SendTextAsync(request, cancellationToken);

    return Results.Ok();
});

app.MapGet("/", () => "Demo.Webhooks is running");

app.Run();

// Expose Program class for integration tests
public partial class Program { }
