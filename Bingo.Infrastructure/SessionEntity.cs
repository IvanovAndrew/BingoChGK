using Azure;
using Azure.Data.Tables;

namespace Bingo.Infrastructure;

public class SessionEntity : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public string Command { get; set; } = default!;
    public int Bingo { get; set; } = -1;

    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    
}