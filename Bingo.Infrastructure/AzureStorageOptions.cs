namespace Bingo.Infrastructure;

public class AzureStorageOptions
{
    public string ConnectionString { get; set; } = default!;
    public string TableName { get; set; } = default!;
}