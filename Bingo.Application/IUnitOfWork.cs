using Bingo.Domain;

namespace Bingo.Application;

public interface IUnitOfWork
{
    void Register(AggregateRoot aggregate);
    Task CommitAsync(CancellationToken cancellationToken = default);
}