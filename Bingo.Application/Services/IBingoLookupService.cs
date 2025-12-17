using Bingo.Domain;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.Services;

public interface IBingoLookupService
{
    Task<BingoLookupResult> LookupAsync(
        int? bingoId, 
        string? text,
        CancellationToken cancellationToken);
}

public record BingoLookupResult(Domain.Bingo? Bingo)
{
    public bool NotFound => Bingo == null;
}

public class BingoLookupService(
    IBingoRepository bingoRepository,
    ILogger<BingoLookupService> logger)
    : IBingoLookupService
{
    public async Task<BingoLookupResult> LookupAsync(
        int? bingoId,
        string? text,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Lookup started: id={id}, text={text}", bingoId, text);

        Domain.Bingo? bingo = null;

        if (bingoId is not null)
        {
            bingo = await bingoRepository.GetBingoByID(bingoId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(text))
        {
            bingo = await bingoRepository.GetBingoByText(text);
        }

        return new BingoLookupResult(bingo);
    }
}
