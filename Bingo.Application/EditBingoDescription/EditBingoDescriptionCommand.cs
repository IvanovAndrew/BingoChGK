using MediatR;

namespace Bingo.Application.EditBingoDescription;

public class EditBingoDescriptionCommand : IRequest
{
    public long ChatId { get; init; }
    public int MessageId { get; init; }
    public int BingoId { get; init; }
    public string Description { get; init; }
}