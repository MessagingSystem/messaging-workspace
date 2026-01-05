namespace Demo.Webhooks;

/// <summary>
/// DTO для запроса отправки файла через endpoint.
/// </summary>
public sealed record SendFileRequestDto
{
    /// <summary>
    /// Идентификатор чата получателя.
    /// </summary>
    public required string ChatId { get; init; }

    /// <summary>
    /// Тип файла (Document, Image, Video, Audio).
    /// </summary>
    public required string Kind { get; init; }

    /// <summary>
    /// Режим отправки файла (FileId, Url, Upload).
    /// </summary>
    public required string Mode { get; init; }

    /// <summary>
    /// Значение для режима FileId или Url.
    /// Для FileId - это идентификатор файла в API провайдера.
    /// Для Url - это URL файла.
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// Опциональная подпись к файлу.
    /// </summary>
    public string? Caption { get; init; }

    /// <summary>
    /// Опциональный текст кнопки клавиатуры.
    /// </summary>
    public string? KeyboardButtonText { get; init; }

    /// <summary>
    /// Опциональные данные callback для кнопки клавиатуры.
    /// </summary>
    public string? KeyboardButtonData { get; init; }
}

/// <summary>
/// Режим отправки файла.
/// </summary>
public enum FileSendMode
{
    /// <summary>
    /// Файл передается по идентификатору в API провайдера.
    /// </summary>
    FileId,

    /// <summary>
    /// Файл передается по URL.
    /// </summary>
    Url,

    /// <summary>
    /// Файл загружается из локального хранилища.
    /// </summary>
    Upload
}
