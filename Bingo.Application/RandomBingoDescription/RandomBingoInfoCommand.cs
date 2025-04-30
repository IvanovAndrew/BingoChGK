using MediatR;

namespace Bingo.Application.RandomBingoDescription;

public class RandomBingoInfoCommand : IRequest
{
    public long ChatId { get; init; }
}