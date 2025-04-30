using Bingo.Application.RandomBingoDescription;
using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.SendRandomBingoToActiveSubscribers;

public class SendRandomBingoToActiveSubscribersHandler(
    IMediator mediator,
    IUserRepository userRepository,
    ILogger<SendRandomBingoToActiveSubscribersHandler> logger)
    : IRequestHandler<SendRandomBingoToActiveSubscribersCommand>
{
    private readonly ILogger _logger = logger;

    public async Task Handle(SendRandomBingoToActiveSubscribersCommand domainEvent, CancellationToken cancellationToken)
    {
        var activeUsers = await userRepository.GetActiveUsers();
        
        _logger.LogInformation($"{nameof(SendRandomBingoToActiveSubscribersHandler)}: ({string.Join(", ", activeUsers.Select(u => u.Id))})");

        foreach (var user in activeUsers)
        {
            await mediator.Send(new RandomBingoInfoCommand() { ChatId = user.Id }, cancellationToken);
        }
        
        _logger.LogInformation($"{nameof(SendRandomBingoToActiveSubscribersHandler)}: ({string.Join(", ", activeUsers.Select(u => u.Id))})");
    }
}