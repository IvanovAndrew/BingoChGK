using Bingo.Domain;
using Bingo.Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

public class BingoRepository : IBingoRepository
{
    private readonly Supabase.Client _db;
    public BingoRepository(Supabase.Client db)
    {
        _db = db;
    }

    public async Task<List<Domain.Bingo>> GetAllBingos()
    {
        var dbResponse = await _db.From<BingoDb>().Get();

        return dbResponse.Models.Select(BingoMapper.ToDomain).ToList();
    }

    public async ValueTask<Domain.Bingo?> GetBingoByID(int bingoId)
    {
        var dbResult = await _db.From<BingoDb>().Where(b => b.Id == bingoId).Get();

        if (dbResult.Model != null)
        {
            var bingo = BingoMapper.ToDomain(dbResult.Model);

            var questions = await _db.From<QuestionDb>().Where(q => q.BingoId == bingoId).Get();
            bingo.AddQuestions(questions.Models.Select(QuestionMapper.ToDomain).ToList());
        }

        return null;
    }

    public async Task<Domain.Bingo?> GetBingoByText(string bingoText)
    {
        var dbResult = await _db.From<BingoDb>().Where(b => b.Bingo == bingoText).Get();

        if (dbResult.Model != null)
        {
            var bingo = BingoMapper.ToDomain(dbResult.Model);

            var questions = await _db.From<QuestionDb>().Where(q => q.BingoId == bingo.Id).Get();
            bingo.AddQuestions(questions.Models.Select(QuestionMapper.ToDomain).ToList());
        }

        return null;
    }

    public async Task SaveBingo(Domain.Bingo bingo)
    {
        await _db.From<BingoDb>().Insert(BingoMapper.ToDbModel(bingo));
    }

    public async ValueTask UpdateBingo(Domain.Bingo bingo)
    {
        await _db.From<BingoDb>().Update(BingoMapper.ToDbModel(bingo));
    }
}

public class BingoRepositoryCacheDecorator : IBingoRepository
{
    private readonly IBingoRepository _source;
    private readonly ILogger<BingoRepositoryCacheDecorator> _logger;

    private const string AllBingosKey = nameof(AllBingosKey);

    public BingoRepositoryCacheDecorator(IBingoRepository source, ILogger<BingoRepositoryCacheDecorator> logger)
    {
        _source = source;
        _logger = logger;
    }
    
    public async Task<List<Domain.Bingo>> GetAllBingos()
    {
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dictionary))
        {
            _logger.LogInformation("Taking all bingos from the cache");
            return dictionary.Values.ToList();
        }

        var allBingos = await _source.GetAllBingos();
        _logger.LogInformation("All bingos are loaded from db");

        Cache.Instance.Set(AllBingosKey, allBingos.ToDictionary(b => b.Id, b => b));

        return allBingos;
    }

    public async ValueTask<Domain.Bingo?> GetBingoByID(int bingoId)
    {
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dictionary)
            && dictionary.TryGetValue(bingoId, out var bingo))
        {
            _logger.LogInformation($"Bingo {bingoId} is taken from the cache");
            return bingo;
        }

        var bingoById = await _source.GetBingoByID(bingoId);
        _logger.LogInformation($"Bingo {bingoId} is taken from the db");
        
        if (dictionary != null)
        {
            dictionary[bingoId] = bingoById;
        }

        return bingoById;
    }

    public async Task<Domain.Bingo?> GetBingoByText(string text)
    {
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dictionary))
        {
            var result = dictionary.Values.FirstOrDefault(b => b.Text == text);
            if (result != null)
                return result;
        }

        var bingo = await _source.GetBingoByText(text);

        if (dictionary != null && bingo != null)
        {
            dictionary[bingo.Id] = bingo;
        }

        return bingo;
    }

    public async Task SaveBingo(Domain.Bingo bingo)
    {
        await _source.SaveBingo(bingo);

        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dict))
        {
            dict[bingo.Id] = bingo;
        }
    }

    public async ValueTask UpdateBingo(Domain.Bingo bingo)
    {
        await _source.UpdateBingo(bingo);
        
        if (Cache.Instance.TryGetValue(AllBingosKey, out Dictionary<int, Domain.Bingo> dict))
        {
            dict[bingo.Id] = bingo;
        }
    }
}