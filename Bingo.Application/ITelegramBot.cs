namespace Bingo.Application;

public interface ITelegramBot
{
    Task<ITelegramMessage> SendTextMessageAsync(long chatId, string text, TelegramButton[]? buttons = null,
        int? replyToMessageId = null,
        bool useMarkdown = false,
        CancellationToken cancellationToken = default);
    
    Task<ITelegramMessage> EditTextMessageAsync(long chatId, int messageId, string text, TelegramButton[]? buttons = null,
        bool useMarkdown = false,
        CancellationToken cancellationToken = default);

    Task EditMessageReplyMarkupAsync(long messageChatId, int messageId, TelegramButton[]? buttons,
        CancellationToken cancellationToken = default);

    Task DeleteMessageAsync(long messageChatId, int messageId);
    Task<ITelegramMessage> ForwardMessageAsync(long chatId, Uri message, CancellationToken cancellationToken = default);
    //Task SetMyCommandsAsync(BotCommand[] botCommands);

    Task<ITelegramMessage> SendPhotoAsync(long chatId, Uri pictireUri, string? caption = null, bool useMarkdown = false,
        TelegramButton[]? buttons = null, CancellationToken cancellationToken = default);
}

public interface ITelegramMessage
{
    public long ChatId { get; }
    public int MessageId { get; }
}

public class TelegramMessage : ITelegramMessage
{
    public long ChatId { get; init; }
    public int MessageId { get; init; }
}