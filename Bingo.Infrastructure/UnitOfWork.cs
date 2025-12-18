using Bingo.Application;
using Bingo.Domain;
using MediatR;

namespace Bingo.Infrastructure;

public class UnitOfWork(IMediator mediator) : IUnitOfWork
{
    private readonly List<AggregateRoot> _tracked = new();

    public void Register(AggregateRoot aggregate)
    {
        if (!_tracked.Contains(aggregate))
            _tracked.Add(aggregate);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        foreach (var aggregate in _tracked)
        {
            foreach (var domainEvent in aggregate.GetEvents())
                await mediator.Publish(domainEvent, cancellationToken);

            aggregate.ClearEvents();
        }
    }
}