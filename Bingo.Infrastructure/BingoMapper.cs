using Bingo.Domain;
using Bingo.Infrastructure.Database;

namespace Bingo.Infrastructure;

public static class BingoMapper
{
    public static Domain.Bingo ToDomain(BingoDb bingoDb)
    {
        return new BingoBuilder(bingoDb.Id, bingoDb.Bingo, bingoDb.Description)
            .WithInstantViewUrl(bingoDb.InstantViewUrl)
            .Build();
    }

    public static BingoDb ToDbModel(Domain.Bingo bingo)
    {
        return new BingoDb()
        {
            Id = bingo.Id,
            Bingo = bingo.Text,
            Description = bingo.Description,
            InstantViewUrl = bingo.InstantViewUrl
        };
    }
}