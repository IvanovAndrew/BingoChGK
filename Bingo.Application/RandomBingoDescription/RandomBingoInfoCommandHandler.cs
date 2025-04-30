using Bingo.Application.GetBingoDescription;
using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.RandomBingoDescription;

public class RandomBingoInfoCommandHandler(
    IMediator mediator,
    IBingoRepository bingoRepository,
    IUserRepository userRepository,
    ILogger<RandomBingoInfoCommandHandler> logger)
    : IRequestHandler<RandomBingoInfoCommand>
{
    private readonly ILogger _logger = logger;

    public async Task Handle(RandomBingoInfoCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(RandomBingoInfoCommandHandler));
        
        var bingos = await bingoRepository.GetAllBingos();
        
        var userTrainingSession = await userRepository.GetUserSession(command.ChatId);
        var bingo = userTrainingSession.GetRandomBingo(bingos);

        await mediator.Send(new GetBingoDescriptionCommand(){BingoId = bingo.Id, ChatId = command.ChatId}, cancellationToken);
    }
}