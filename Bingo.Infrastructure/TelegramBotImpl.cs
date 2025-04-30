using Bingo.Application;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bingo.Infrastructure;

public class TelegramBotImpl : ITelegramBot
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramBotImpl(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));
        
        _telegramBotClient = new TelegramBotClient(token);
    }

    public async Task<ITelegramMessage> SendTextMessageAsync(long chatId, string text, TelegramButton[]? buttons = null,
        int? replyToMessageId = null,
        bool useMarkdown = false,
        CancellationToken cancellationToken = default)
    {
        var message = await _telegramBotClient.SendMessage(chatId, text, replyMarkup: MapButtons(buttons),
            replyParameters: replyToMessageId != null? new ReplyParameters(){ChatId = chatId, MessageId = replyToMessageId.Value} : null,
            parseMode: useMarkdown ? ParseMode.MarkdownV2 : ParseMode.None, cancellationToken: cancellationToken);
        return new TelegramMessage { ChatId = message.Chat.Id, MessageId = message.MessageId };
    }

    public async Task<ITelegramMessage> EditTextMessageAsync(long chatId, int messageId, string text, TelegramButton[]? buttons = null, 
        bool useMarkdown = false, CancellationToken cancellationToken = default)
    {
        var message = await _telegramBotClient.EditMessageText(chatId, messageId, text, replyMarkup: MapButtons(buttons),
            parseMode: useMarkdown ? ParseMode.MarkdownV2 : ParseMode.None, cancellationToken: cancellationToken);
        return new TelegramMessage { ChatId = message.Chat.Id, MessageId = message.MessageId };
    }

    public async Task EditMessageReplyMarkupAsync(long messageChatId, int messageId, TelegramButton[]? buttons,
        CancellationToken cancellationToken = default)
    {
        await _telegramBotClient.EditMessageReplyMarkup(messageChatId, messageId, replyMarkup: MapButtons(buttons),
            cancellationToken: cancellationToken);
    }

    public async Task DeleteMessageAsync(long chatId, int messageId)
    {
        await _telegramBotClient.DeleteMessage(chatId, messageId);
    }

    public async Task<ITelegramMessage> ForwardMessageAsync(long chatId, Uri message,
        CancellationToken cancellationToken = default)
    {
        var chatFrom = $"@{message.Segments[^2]}";
        chatFrom = chatFrom.Replace("/", string.Empty);
        var messageId = int.Parse(message.Segments[^1]);
        var sentMessage = await _telegramBotClient.ForwardMessage(new ChatId(chatId), new ChatId(chatFrom),
            messageId,
            cancellationToken: cancellationToken);

        return new TelegramMessage() { ChatId = sentMessage.Chat.Id, MessageId = sentMessage.MessageId };
    }

    public async Task SetMyCommandsAsync(BotCommand[] botCommands)
    {
        await _telegramBotClient.SetMyCommands(botCommands);
    }

    public async Task<ITelegramMessage> SendPhotoAsync(long chatId, Uri pictireUri, string? caption = null,
        bool useMarkdown = false,
        TelegramButton[]? buttons = null, CancellationToken cancellationToken = default)
    {
        var sentMessage = await _telegramBotClient.SendPhoto(chatId, new InputFileUrl(pictireUri),
            caption: caption,
            parseMode: useMarkdown ? ParseMode.MarkdownV2 : ParseMode.None,
            replyMarkup: MapButtons(buttons),
            cancellationToken: cancellationToken);

        return new TelegramMessage() { ChatId = sentMessage.Chat.Id, MessageId = sentMessage.MessageId };
    }

    private InlineKeyboardMarkup? MapButtons(TelegramButton[]? buttons, int buttonsPerRow = 2)
    {
        if (buttons != null && buttons.Any())
        {
            return 
                new InlineKeyboardMarkup(
                    new[]
                    {
                        buttons.Take(buttonsPerRow).Select(b =>
                            InlineKeyboardButton.WithCallbackData(b.Text, b.Callback)),
                        buttons.Skip(buttonsPerRow).Take(buttonsPerRow).Select(b =>
                            InlineKeyboardButton.WithCallbackData(b.Text, b.Callback)),
                    });
        }

        return null;
    }
}