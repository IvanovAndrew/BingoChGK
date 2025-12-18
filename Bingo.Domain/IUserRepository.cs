namespace Bingo.Domain;

public interface IUserRepository
{
    Task<List<User>> GetActiveUsers();
    Task CreateUser(User user);
    Task<User?> GetUserById(long chatId);
    Task<UserTrainingSession> GetUserSession(long chatId);
    Task SaveSession(long chatId, int bingoId, DateTime date, CancellationToken cancellationToken);
    Task Update(User user);
}