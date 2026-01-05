using Messaging.Abstractions;

namespace Demo.Webhooks;

/// <summary>
/// Реализация фабрики для получения клиентов мессенджеров по имени провайдера.
/// Использует keyed services из DI контейнера.
/// </summary>
public sealed class MessagingClientFactory : IMessagingClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MessagingClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IChatClient GetChatClient(string provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Provider name cannot be empty.", nameof(provider));
        }

        var client = _serviceProvider.GetKeyedService<IChatClient>(provider.ToLowerInvariant());

        if (client is null)
        {
            throw new ArgumentException(
                $"Provider '{provider}' is not registered. Available providers: telegram, max.",
                nameof(provider));
        }

        return client;
    }

    public IChatFilesClient GetChatFilesClient(string provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Provider name cannot be empty.", nameof(provider));
        }

        var client = _serviceProvider.GetKeyedService<IChatFilesClient>(provider.ToLowerInvariant());

        if (client is null)
        {
            throw new ArgumentException(
                $"Provider '{provider}' is not registered. Available providers: telegram, max.",
                nameof(provider));
        }

        return client;
    }
}
