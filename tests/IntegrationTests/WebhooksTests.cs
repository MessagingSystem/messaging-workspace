using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace IntegrationTests;

public class WebhooksTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task PostWebhookTelegram_ReturnsOk()
    {
        // Arrange
        var webhookPayload = """
        {
            "update_id": 123456789,
            "message": {
                "message_id": 1,
                "from": {
                    "id": 987654321,
                    "is_bot": false,
                    "first_name": "Test",
                    "username": "testuser"
                },
                "chat": {
                    "id": 987654321,
                    "first_name": "Test",
                    "username": "testuser",
                    "type": "private"
                },
                "date": 1234567890,
                "text": "Hello, bot!"
            }
        }
        """;

        var content = new StringContent(webhookPayload, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/webhooks/telegram", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task PostWebhookTelegramCallback_LogsCallbackData()
    {
        // Arrange
        var logProvider = new InMemoryLoggerProvider();
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddProvider(logProvider);
                });
            });
        using var client = factory.CreateClient();

        var webhookPayload = """
        {
            "update_id": 123456789,
            "callback_query": {
                "id": "callback-1",
                "data": "ok",
                "message": {
                    "message_id": 1,
                    "chat": {
                        "id": 987654321
                    }
                }
            }
        }
        """;

        var content = new StringContent(webhookPayload, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/webhooks/telegram", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(
            logProvider.Entries.Any(entry => entry.Message == "Callback data: ok"),
            Is.True);
    }

    private sealed record LogEntry(string Category, LogLevel Level, string Message);

    private sealed class InMemoryLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<LogEntry> _entries = new();

        public IReadOnlyCollection<LogEntry> Entries => _entries.ToArray();

        public ILogger CreateLogger(string categoryName)
            => new InMemoryLogger(categoryName, _entries);

        public void Dispose()
        {
        }
    }

    private sealed class InMemoryLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly ConcurrentQueue<LogEntry> _entries;

        public InMemoryLogger(string categoryName, ConcurrentQueue<LogEntry> entries)
        {
            _categoryName = categoryName;
            _entries = entries;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            _entries.Enqueue(new LogEntry(_categoryName, logLevel, message));
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}
