using Bingo.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

public class BingoRepositoryCacheDecorator(IBingoRepository source, ILogger<BingoRepositoryCacheDecorator> logger)
    : IBingoRepository
{
    private const string AllBingosKey = nameof(AllBingosKey);

    public async Task<List<Domain.Bingo>> GetAllBingos()
    {
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dictionary))
        {
            logger.LogInformation("Taking all bingos from the cache");
            return dictionary.Values.ToList();
        }

        var allBingos = await source.GetAllBingos();
        logger.LogInformation("All bingos are loaded from db");

        Cache.Instance.Set(AllBingosKey, allBingos.ToDictionary(b => b.Id, b => b));

        return allBingos;
    }

    public async ValueTask<Domain.Bingo?> GetBingoByID(int bingoId)
    {
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dictionary)
            && dictionary.TryGetValue(bingoId, out var bingo))
        {
            logger.LogInformation($"Bingo {bingoId} is taken from the cache");

            return bingo;
        }

        var bingoById = await source.GetBingoByID(bingoId);
        logger.LogInformation($"Bingo {bingoId} is taken from the db ({bingoById?.ToString()})");
        
        if (dictionary != null)
        {
            dictionary[bingoId] = bingoById;
        }

        return bingoById;
    }

    public async Task<Domain.Bingo?> GetBingoByText(string text)
    {
        logger.LogInformation($"{nameof(GetBingoByText)} started ({text})");
        
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dictionary))
        {
            var result = dictionary.Values.FirstOrDefault(b => b.Text == text);
            
            logger.LogInformation($"{nameof(GetBingoByText)} checking cache. Result is {result?.Id}");
            if (result != null)
                return result;
        }

        logger.LogInformation($"{nameof(GetBingoByText)} getting value {text} from db");
        var bingo = await source.GetBingoByText(text);
        
        logger.LogInformation($"{nameof(GetBingoByText)} db value is {bingo}");

        if (dictionary != null && bingo != null)
        {
            dictionary[bingo.Id] = bingo;
        }

        return bingo;
    }

    public async Task<List<string>> GetBingoSynonyms(int bingoId)
    {
        return await source.GetBingoSynonyms(bingoId);
    }

    public async Task SaveBingo(Domain.Bingo bingo)
    {
        await source.SaveBingo(bingo);

        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dict))
        {
            dict[bingo.Id] = bingo;
        }
    }

    public async ValueTask UpdateBingo(Domain.Bingo bingo)
    {
        await source.UpdateBingo(bingo);
        
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dict))
        {
            dict[bingo.Id] = bingo;
        }
    }
}