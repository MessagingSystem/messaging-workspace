using Messaging.Hosting.AspNetCore;
using Messaging.Max;
using Messaging.Telegram;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Webhooks;

/// <summary>
/// Обработчик входящих webhook-обновлений.
/// Логирует информацию о провайдере и размере данных.
/// </summary>
public sealed class WebhookUpdateHandler : IWebhookUpdateHandler
{
    private readonly ILogger<WebhookUpdateHandler> _logger;
    private readonly IServiceProvider _services;
    private readonly TelegramCallbackUpdateParser _telegramCallbackParser = new();
    private readonly MaxCallbackUpdateParser _maxCallbackParser = new();

    public WebhookUpdateHandler(ILogger<WebhookUpdateHandler> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    public async Task HandleAsync(WebhookEnvelope envelope, CancellationToken cancellationToken)
    {
        var provider = envelope.Provider;
        var rawJsonSize = envelope.RawJson.Length;

        _logger.LogInformation(
            "Received webhook from provider '{Provider}', RawJson size: {Size} bytes",
            provider,
            rawJsonSize);

        if (string.Equals(provider, "telegram", StringComparison.OrdinalIgnoreCase))
        {
            await HandleTelegramCallbackAsync(envelope.RawJson, cancellationToken);
            return;
        }

        if (string.Equals(provider, "max", StringComparison.OrdinalIgnoreCase))
        {
            HandleMaxCallback(envelope.RawJson);
        }
    }

    private async Task HandleTelegramCallbackAsync(string rawJson, CancellationToken cancellationToken)
    {
        if (!_telegramCallbackParser.TryParseCallback(rawJson, out var callback))
        {
            return;
        }

        _logger.LogInformation("Callback data: {Data}", callback.Data);

        var acknowledger = _services.GetService<ITelegramCallbackAcknowledger>();
        if (acknowledger is not null)
        {
            await acknowledger.AnswerAsync(callback.CallbackId, ct: cancellationToken);
        }
    }

    private void HandleMaxCallback(string rawJson)
    {
        if (!_maxCallbackParser.TryParseCallback(rawJson, out var callback))
        {
            return;
        }

        _logger.LogInformation("Callback data: {Data}", callback.Data);
    }
}
