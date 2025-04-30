using Bingo.Domain;
using Bingo.Infrastructure.Database;

namespace Bingo.Infrastructure;

public static class UserMapper
{
    public static User ToDomain(SubscriberDb subscriberDb)
    {
        return new User()
        {
            Id = subscriberDb.Id,
            CanAddBingo = subscriberDb.IsAdmin,
            CanRemoveQuestion = subscriberDb.IsAdmin
        };
    }
    
    public static SubscriberDb FromDomain(Domain.User user)
    {
        return new SubscriberDb()
        {
            Id = user.Id,
            IsActive = true,
            IsAdmin = user.CanAddBingo || user.CanRemoveQuestion
        };
    }
}