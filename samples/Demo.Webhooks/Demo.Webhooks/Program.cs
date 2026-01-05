using Demo.Webhooks;
using Messaging.Abstractions;
using Messaging.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация Messaging Hosting
builder.Services.AddMessagingHosting();

// Регистрация обоих провайдеров одновременно
builder.Services.AddMultipleMessagingProviders(
    configureTelegram: opts =>
    {
        opts.BotToken = builder.Configuration["Telegram:BotToken"] ?? "demo-bot-token";
    },
    configureMax: opts =>
    {
        opts.AccessToken = builder.Configuration["Max:AccessToken"] ?? "demo-access-token";
        opts.BaseUrl = new Uri(builder.Configuration["Max:BaseUrl"] ?? "https://platform-api.max.ru");
    });

// Регистрация фабрики клиентов
builder.Services.AddMessagingClientFactory();

// Регистрация обработчика webhook'ов
builder.Services.AddSingleton<IWebhookUpdateHandler, WebhookUpdateHandler>();

var app = builder.Build();

// Маппинг endpoint'ов для webhook'ов
app.MapMessagingWebhooks();

app.MapGet("/demo/send-keyboard/{provider}", async (
    string provider,
    string? chatId,
    IMessagingClientFactory factory,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(chatId))
    {
        return Results.BadRequest("chatId query parameter is required.");
    }

    try
    {
        var chatClient = factory.GetChatClient(provider);

        var keyboard = new InlineKeyboard(new[]
        {
            new InlineKeyboardRow(new InlineKeyboardButton[]
            {
                new CallbackButton("OK", "ok"),
                new UrlButton("Docs", new Uri("https://example.com/docs"))
            })
        });

        await Message.To(new ChatId(chatId))
            .Text($"Inline keyboard demo ({provider})")
            .Keyboard(keyboard)
            .SendAsync(chatClient, cancellationToken);

        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/demo/send-file/{provider}", async (
    string provider,
    SendFileRequestDto dto,
    IMessagingClientFactory factory,
    CancellationToken cancellationToken) =>
{
    // Валидация входных данных
    if (string.IsNullOrWhiteSpace(dto.ChatId))
    {
        return Results.BadRequest("chatId is required.");
    }

    if (!Enum.TryParse<FileKind>(dto.Kind, true, out var fileKind))
    {
        return Results.BadRequest($"Invalid kind. Valid values: {string.Join(", ", Enum.GetNames<FileKind>())}");
    }

    if (!Enum.TryParse<FileSendMode>(dto.Mode, true, out var mode))
    {
        return Results.BadRequest($"Invalid mode. Valid values: {string.Join(", ", Enum.GetNames<FileSendMode>())}");
    }

    try
    {
        var client = factory.GetChatFilesClient(provider);

        // Создание InputFile в зависимости от режима
        InputFile inputFile = mode switch
        {
            FileSendMode.FileId => new FileId(dto.Value ?? throw new ArgumentException("value is required for fileId mode.")),
            FileSendMode.Url => new RemoteUrl(dto.Value ?? throw new ArgumentException("value is required for url mode.")),
            FileSendMode.Upload => throw new NotImplementedException(
                "Upload mode requires file stream. For MAX provider, real upload requires access to MAX API. " +
                "For testing, use fileId or url mode."),
            _ => throw new ArgumentException($"Unsupported mode: {mode}")
        };

        // Построение и отправка сообщения
        var builder = Message.To(new ChatId(dto.ChatId))
            .File(inputFile, fileKind);

        if (!string.IsNullOrWhiteSpace(dto.Caption))
        {
            builder = builder.Caption(dto.Caption);
        }

        if (!string.IsNullOrWhiteSpace(dto.KeyboardButtonText))
        {
            var keyboard = new InlineKeyboard(new[]
            {
                new InlineKeyboardRow(new InlineKeyboardButton[]
                {
                    new CallbackButton(dto.KeyboardButtonText, dto.KeyboardButtonData ?? "callback_data")
                })
            });
            builder = builder.Keyboard(keyboard);
        }

        var result = await builder.SendAsync(client, cancellationToken);

        return Results.Ok(new { messageId = result.MessageId.Value });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (NotImplementedException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (MessagingException ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: $"Messaging error: {ex.Code}");
    }
});

app.MapGet("/", () => "Demo.Webhooks is running");

app.Run();

// Expose Program class for integration tests
public partial class Program { }
