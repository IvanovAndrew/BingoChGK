using MediatR;

namespace Bingo.Application.GetBingoQuestion;

public record GetBingoQuestionCommand : IRequest
{
    public int BingoId { get; init; }
    public long ChatId { get; init; }
    public bool RetryIfNoQuestions { get; init; } = true;
}