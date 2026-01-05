using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

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
}
