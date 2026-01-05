using Demo.Webhooks;
using Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

/// <summary>
/// Интеграционные тесты для проверки регистрации провайдеров и фабрики клиентов.
/// </summary>
public class MessagingClientFactoryTests
{
    private WebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
    }

    [Test]
    public void Factory_IsRegisteredInDI()
    {
        // Arrange & Act
        var factory = _factory.Services.GetService<IMessagingClientFactory>();

        // Assert
        Assert.That(factory, Is.Not.Null, "IMessagingClientFactory should be registered in DI.");
    }

    [Test]
    public void Factory_CanResolveTelegramChatClient()
    {
        // Arrange
        var factory = _factory.Services.GetRequiredService<IMessagingClientFactory>();

        // Act
        var client = factory.GetChatClient("telegram");

        // Assert
        Assert.That(client, Is.Not.Null, "Telegram IChatClient should be resolvable via factory.");
    }

    [Test]
    public void Factory_CanResolveTelegramChatFilesClient()
    {
        // Arrange
        var factory = _factory.Services.GetRequiredService<IMessagingClientFactory>();

        // Act
        var client = factory.GetChatFilesClient("telegram");

        // Assert
        Assert.That(client, Is.Not.Null, "Telegram IChatFilesClient should be resolvable via factory.");
    }

    [Test]
    public void Factory_CanResolveMaxChatClient()
    {
        // Arrange
        var factory = _factory.Services.GetRequiredService<IMessagingClientFactory>();

        // Act
        var client = factory.GetChatClient("max");

        // Assert
        Assert.That(client, Is.Not.Null, "MAX IChatClient should be resolvable via factory.");
    }

    [Test]
    public void Factory_CanResolveMaxChatFilesClient()
    {
        // Arrange
        var factory = _factory.Services.GetRequiredService<IMessagingClientFactory>();

        // Act
        var client = factory.GetChatFilesClient("max");

        // Assert
        Assert.That(client, Is.Not.Null, "MAX IChatFilesClient should be resolvable via factory.");
    }

    [Test]
    public void Factory_ThrowsForUnknownProvider()
    {
        // Arrange
        var factory = _factory.Services.GetRequiredService<IMessagingClientFactory>();

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => factory.GetChatClient("unknown"));
        Assert.That(ex!.Message, Does.Contain("not registered"));
    }

    [Test]
    public void Factory_IsCaseInsensitive()
    {
        // Arrange
        var factory = _factory.Services.GetRequiredService<IMessagingClientFactory>();

        // Act
        var telegramUpper = factory.GetChatClient("TELEGRAM");
        var telegramLower = factory.GetChatClient("telegram");
        var maxMixed = factory.GetChatClient("MaX");

        // Assert
        Assert.That(telegramUpper, Is.Not.Null);
        Assert.That(telegramLower, Is.Not.Null);
        Assert.That(maxMixed, Is.Not.Null);
    }
}
