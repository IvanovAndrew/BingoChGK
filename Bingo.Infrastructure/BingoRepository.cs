using Bingo.Domain;
using Bingo.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

public class BingoRepository(Supabase.Client db, ILogger<BingoRepository> logger) : IBingoRepository
{
    public async Task<List<Domain.Bingo>> GetAllBingos()
    {
        var dbResponse = await db.From<BingoDb>().Get();

        return dbResponse.Models.Select(BingoMapper.ToDomain).ToList();
    }

    public async ValueTask<Domain.Bingo?> GetBingoByID(int bingoId)
    {
        logger.LogInformation($"Repository: getting bingo by Id {bingoId}");
        var dbResult = await db.From<BingoDb>().Where(b => b.Id == bingoId).Get();

        if (dbResult.Model == null) return null;
        
        var bingo = BingoMapper.ToDomain(dbResult.Model);

        return bingo;
    }

    public async Task<Domain.Bingo?> GetBingoByText(string bingoText)
    {
        var dbResult = await db.From<BingoDb>().Where(b => b.Bingo == bingoText).Get();

        if (dbResult.Model == null)
        {
            var dbId = await db.From<BingoSynonymDb>().Where(s => s.Synonym == bingoText).Get();

            if (dbId.Model != null)
            {
                return await GetBingoByID(dbId.Model.BingoId);
            }

            return null;
        }
        
        var bingo = BingoMapper.ToDomain(dbResult.Model);
        return bingo;
    }

    public async Task<List<string>> GetBingoSynonyms(int bingoId)
    {
        var dbResponse = await db.From<BingoSynonymDb>().Where(s => s.BingoId == bingoId).Get();
        
        return dbResponse.Models.Select(m => m.Synonym).ToList();
    }

    public async Task SaveBingo(Domain.Bingo bingo)
    {
        await db.From<BingoDb>().Insert(BingoMapper.ToDbModel(bingo));
    }

    public async ValueTask UpdateBingo(Domain.Bingo bingo)
    {
        await db.From<BingoDb>().Update(BingoMapper.ToDbModel(bingo));
    }
}