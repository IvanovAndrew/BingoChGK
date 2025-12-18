using Bingo.Domain;
using Bingo.Infrastructure.Database;

namespace Bingo.Infrastructure;

public static class UserMapper
{
    public static User ToDomain(SubscriberDb subscriberDb)
    {
        return new User(subscriberDb.Id, subscriberDb.IsActive, subscriberDb.IsAdmin);
    }
    
    public static SubscriberDb FromDomain(User user)
    {
        return new SubscriberDb()
        {
            Id = user.Id,
            IsActive = user.IsActive,
            IsAdmin = user.CanAddBingo || user.CanRemoveQuestion
        };
    }
}