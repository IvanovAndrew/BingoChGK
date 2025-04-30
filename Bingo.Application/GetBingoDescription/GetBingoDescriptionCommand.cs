using MediatR;

namespace Bingo.Application.GetBingoDescription;

public class GetBingoDescriptionCommand : IRequest
{
    public long ChatId { get; init; }
    public int? MessageId { get; init; }
    public int? BingoId { get; init; }
    public string? Text { get; init; }
}