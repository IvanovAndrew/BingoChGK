namespace Bingo.Domain;

public interface IBingoRepository
{
    Task<List<Bingo>> GetAllBingos();
    ValueTask<Bingo?> GetBingoByID(int bingoId);
    Task<Bingo?> GetBingoByText(string bingo);
    Task<List<string>> GetBingoSynonyms(int bingoId);
    Task SaveBingo(Bingo bingo);
    ValueTask UpdateBingo(Bingo bingo);
}