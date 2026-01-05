using Messaging.Hosting.AspNetCore;

namespace Demo.Webhooks;

/// <summary>
/// Обработчик входящих webhook-обновлений.
/// Логирует информацию о провайдере и размере данных.
/// </summary>
public sealed class WebhookUpdateHandler : IWebhookUpdateHandler
{
    private readonly ILogger<WebhookUpdateHandler> _logger;

    public WebhookUpdateHandler(ILogger<WebhookUpdateHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(WebhookEnvelope envelope, CancellationToken cancellationToken)
    {
        var provider = envelope.Provider;
        var rawJsonSize = envelope.RawJson.Length;

        _logger.LogInformation(
            "Received webhook from provider '{Provider}', RawJson size: {Size} bytes",
            provider,
            rawJsonSize);

        return Task.CompletedTask;
    }
}
