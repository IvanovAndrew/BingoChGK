namespace Bingo.Application;

public interface IConversationFlowManager
{
    Task SetStepAsync(long chatId, string command, int bingoId);
    Task<(string? Command, int BingoId)> GetStepAsync(long chatId);
    Task ClearStepAsync(long chatId);
}

public class ConversationFlowManager(ISessionService sessionService) : IConversationFlowManager
{
    public async Task SetStepAsync(long chatId, string command, int bingoId)
    {
        await sessionService.SaveStepAsync(chatId, command, bingoId);
    }

    public async Task<(string? Command, int BingoId)> GetStepAsync(long chatId)
    {
        return await sessionService.GetStepAsync(chatId);
    }

    public async Task ClearStepAsync(long chatId)
    {
        await sessionService.ClearAsync(chatId);
    }
}