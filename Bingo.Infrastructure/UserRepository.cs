using Bingo.Application;
using Bingo.Domain;
using Bingo.Infrastructure.Database;

namespace Bingo.Infrastructure;

public class UserRepository(Supabase.Client db, IUnitOfWork unitOfWork) : IUserRepository
{
    public async Task<List<User>> GetActiveUsers()
    {
        var modeledResponse =  await db.From<SubscriberDb>()
            .Where(q => q.IsActive == true)
            .Get();

        return modeledResponse.Models.Select(UserMapper.ToDomain).ToList();
    }

    public async Task CreateUser(User user)
    {
        await db.From<SubscriberDb>().Insert(UserMapper.FromDomain(user));
        unitOfWork.Register(user);
    }

    public async Task<User?> GetUserById(long chatId)
    {
        var subscriber = await db.From<SubscriberDb>().Where(s => s.Id == chatId).Get();

        return subscriber.Model != null? (User?) UserMapper.ToDomain(subscriber.Model) : null;
    }

    public async Task<UserTrainingSession> GetUserSession(long chatId)
    {
        var user = await GetUserById(chatId);

        var dbResponse = await db.From<SessionHistoryDb>().Where(s => s.SubscriberId == chatId).Get();

        return new UserTrainingSession(user, dbResponse.Models.ToDictionary(i => i.BingoId, d => d.LastShownDate));
    }

    public async Task SaveSession(long chatId, int bingoId, DateTime date, CancellationToken cancellationToken)
    {
        if (chatId != 206137342)
        {
            await db.From<SubscriberDb>().Upsert(new SubscriberDb() { Id = chatId, IsActive = true, IsAdmin = false }, cancellationToken: cancellationToken);
        }
        
        await db.From<SessionHistoryDb>().Upsert(new SessionHistoryDb()
            { SubscriberId = chatId, BingoId = bingoId, LastShownDate = date }, cancellationToken: cancellationToken);
    }

    public async Task Update(User user)
    {
        await db.From<SubscriberDb>().Update(UserMapper.FromDomain(user));
        unitOfWork.Register(user);
    }
}