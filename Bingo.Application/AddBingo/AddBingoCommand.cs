using MediatR;

namespace Bingo.Application.AddBingo;

public class AddBingoCommand : IRequest
{
    public long ChatId { get; init; }
    public int MessageToEdit { get; init; }
    public string Text { get; init; }
}