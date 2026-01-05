using Messaging.Abstractions;

namespace Demo.Webhooks;

/// <summary>
/// Фабрика для получения клиентов мессенджеров по имени провайдера.
/// </summary>
public interface IMessagingClientFactory
{
    /// <summary>
    /// Получает клиент для отправки текстовых сообщений по имени провайдера.
    /// </summary>
    /// <param name="provider">Имя провайдера (например, "telegram", "max").</param>
    /// <returns>Клиент для отправки сообщений.</returns>
    /// <exception cref="ArgumentException">Если провайдер не зарегистрирован.</exception>
    IChatClient GetChatClient(string provider);

    /// <summary>
    /// Получает клиент для отправки файлов по имени провайдера.
    /// </summary>
    /// <param name="provider">Имя провайдера (например, "telegram", "max").</param>
    /// <returns>Клиент для отправки файлов.</returns>
    /// <exception cref="ArgumentException">Если провайдер не зарегистрирован.</exception>
    IChatFilesClient GetChatFilesClient(string provider);
}
