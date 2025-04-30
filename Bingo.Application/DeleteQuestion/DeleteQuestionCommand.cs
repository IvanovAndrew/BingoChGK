using MediatR;

namespace Bingo.Application.DeleteQuestion;

public class DeleteQuestionCommand : IRequest
{
    public int BingoID { get; init; }
    public int QuestionID { get; init; }
    public long ChatId { get; init; }
}