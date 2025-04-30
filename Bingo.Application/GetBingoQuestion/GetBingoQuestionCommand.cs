using MediatR;

namespace Bingo.Application.GetBingoQuestion;

public class GetBingoQuestionCommand : IRequest
{
    public int BingoId { get; init; }
    public long ChatId { get; init; }
}