using Bingo.Domain;
using Bingo.Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly Supabase.Client _db;

    public UserRepository(Supabase.Client db)
    {
        _db = db;
    }
    
    public async Task<List<User>> GetActiveUsers()
    {
        var modeledResponse =  await _db.From<SubscriberDb>()
            .Where(q => q.IsActive == true)
            .Get();

        return modeledResponse.Models.Select(UserMapper.ToDomain).ToList();
    }

    public async Task CreateUser(User user)
    {
        await _db.From<SubscriberDb>().Insert(UserMapper.FromDomain(user));
    }

    public async Task<User?> GetUserById(long chatId)
    {
        var subscriber = await _db.From<SubscriberDb>().Where(s => s.Id == chatId).Get();

        return subscriber.Model != null? (User?) UserMapper.ToDomain(subscriber.Model) : null;
    }

    public async Task<UserTrainingSession> GetUserSession(long chatId)
    {
        var user = await GetUserById(chatId);

        var dbResponse = await _db.From<SessionHistoryDb>().Where(s => s.SubscriberId == chatId).Get();

        return new UserTrainingSession(user, dbResponse.Models.ToDictionary(i => i.BingoId, d => d.LastShownDate));
    }

    public async Task SaveSession(long chatId, int bingoId, DateTime date)
    {
        await _db.From<SessionHistoryDb>().Upsert(new SessionHistoryDb()
            { SubscriberId = chatId, BingoId = bingoId, LastShownDate = date });
    }
}

public class UserRepositoryCacheDecorator : IUserRepository
{
    private readonly IUserRepository _source;
    private readonly ILogger<IUserRepository> _logger;

    private const string AllUsersKey = nameof(AllUsersKey);
    private const string TrainingSessionKey = nameof(TrainingSessionKey);

    public UserRepositoryCacheDecorator(IUserRepository source, ILogger<IUserRepository> logger)
    {
        _source = source;
        _logger = logger;
    }

    public async Task<List<User>> GetActiveUsers()
    {
        var cache = GetCachedUsers();

        if (cache.Any())
        {
            return cache.Values.ToList();
        }

        var activeUsers = await _source.GetActiveUsers();

        foreach (var activeUser in activeUsers)
        {
            cache[activeUser.Id] = activeUser;
        }

        return activeUsers;
    }

    public async Task CreateUser(User user)
    {
        await _source.CreateUser(user);

        var cache = GetCachedUsers();
        cache[user.Id] = user;
    }

    public async Task<User?> GetUserById(long chatId)
    {
        var cachedUsers = GetCachedUsers();

        if (cachedUsers.TryGetValue(chatId, out User user))
        {
            return user;
        }
        
        user = await _source.GetUserById(chatId);
        cachedUsers[user.Id] = user;

        return user;
    }

    public async Task<UserTrainingSession> GetUserSession(long chatId)
    {
        var cachedSessions = GetCachedTrainingSessions();

        if (cachedSessions.TryGetValue(chatId, out UserTrainingSession session))
        {
            _logger.LogInformation($"Session with chat {chatId} is taken from the cache");
            return session;
        }
        
        var trainingSession = await _source.GetUserSession(chatId);
        cachedSessions[chatId] = trainingSession;
        
        _logger.LogInformation($"Session with chat {chatId} is taken from the db");

        return trainingSession;
    }

    public async Task SaveSession(long chatId, int bingoId, DateTime date)
    {
        await _source.SaveSession(chatId, bingoId, date);
    }

    private Dictionary<long, User> GetCachedUsers()
    {
        if (Cache.Instance.TryGetValue(AllUsersKey, out var dictionary))
        {
            return dictionary as Dictionary<long, User>;
        }

        var newDictionary = new Dictionary<long, User>();

        Cache.Instance.Set(AllUsersKey, newDictionary);

        return newDictionary;
    }

    private Dictionary<long, UserTrainingSession> GetCachedTrainingSessions()
    {
        if (Cache.Instance.TryGetValue(TrainingSessionKey, out var dictionary))
        {
            return dictionary as Dictionary<long, UserTrainingSession>;
        }

        var newDictionary = new Dictionary<long, UserTrainingSession>();

        Cache.Instance.Set(TrainingSessionKey, newDictionary);

        return newDictionary;
    }
}