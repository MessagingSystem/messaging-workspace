using Messaging.Abstractions;
using Messaging.Max;
using Messaging.Telegram;

namespace Demo.Webhooks;

/// <summary>
/// Расширения для регистрации мессенджер провайдеров с поддержкой нескольких провайдеров одновременно.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует оба провайдера (Telegram и MAX) одновременно с keyed services.
    /// </summary>
    public static IServiceCollection AddMultipleMessagingProviders(
        this IServiceCollection services,
        Action<TelegramOptions> configureTelegram,
        Action<MaxOptions> configureMax)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureTelegram);
        ArgumentNullException.ThrowIfNull(configureMax);

        // Создаем ServiceCollection для каждого провайдера отдельно
        // и регистрируем их как keyed services

        // Регистрируем Telegram провайдер
        var telegramServices = new ServiceCollection();
        telegramServices.AddTelegramMessaging(configureTelegram);
        var telegramProvider = telegramServices.BuildServiceProvider();

        services.AddKeyedSingleton<IChatClient>("telegram", (_, __) =>
            telegramProvider.GetRequiredService<IChatClient>());
        services.AddKeyedSingleton<IChatFilesClient>("telegram", (_, __) =>
            telegramProvider.GetRequiredService<IChatFilesClient>());
        services.AddKeyedSingleton<ITelegramCallbackAcknowledger>("telegram", (_, __) =>
            telegramProvider.GetRequiredService<ITelegramCallbackAcknowledger>());

        // Регистрируем MAX провайдер
        var maxServices = new ServiceCollection();
        maxServices.AddMaxMessaging(configureMax);
        var maxProvider = maxServices.BuildServiceProvider();

        services.AddKeyedSingleton<IChatClient>("max", (_, __) =>
            maxProvider.GetRequiredService<IChatClient>());
        services.AddKeyedSingleton<IChatFilesClient>("max", (_, __) =>
            maxProvider.GetRequiredService<IChatFilesClient>());
        services.AddKeyedSingleton<IMaxWebhookAdmin>("max", (_, __) =>
            maxProvider.GetRequiredService<IMaxWebhookAdmin>());

        // Регистрируем parsers для обработчика webhook'ов
        services.AddSingleton<TelegramCallbackUpdateParser>();
        services.AddSingleton<MaxCallbackUpdateParser>();

        return services;
    }

    /// <summary>
    /// Регистрирует фабрику клиентов для резолвинга по имени провайдера.
    /// </summary>
    public static IServiceCollection AddMessagingClientFactory(this IServiceCollection services)
    {
        services.AddSingleton<IMessagingClientFactory, MessagingClientFactory>();
        return services;
    }
}
