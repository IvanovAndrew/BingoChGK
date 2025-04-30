namespace Bingo.Application;

public interface ISessionService
{
    Task SaveStepAsync(long chatId, string command, int bingoId);
    Task<(string? Command, int BingoId)> GetStepAsync(long chatId);
    Task ClearAsync(long chatId);
}