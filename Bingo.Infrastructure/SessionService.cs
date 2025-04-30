using Azure;
using Azure.Data.Tables;
using Bingo.Application;

namespace Bingo.Infrastructure;

public class AzureTableSessionService : ISessionService
{
    private readonly TableClient _tableClient;

    public AzureTableSessionService(string connectionString, string tableName)
    {
        _tableClient = new TableClient(connectionString, tableName);
        _tableClient.CreateIfNotExists();
    }

    public AzureTableSessionService(AzureStorageOptions options) : this(options.ConnectionString, options.TableName)
    {
        
    }

    public async Task ClearSessionAsync(long chatId)
    {
        try
        {
            await _tableClient.DeleteEntityAsync(chatId.ToString(), "CurrentCard");
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // already deleted
        }
    }

    public async Task SaveStepAsync(long chatId, string command, int bingoId)
    {
        var entity = new SessionEntity()
        {
            PartitionKey = chatId.ToString(),
            RowKey = "CurrentCard",
            Command = command,
            Bingo = bingoId,
            Timestamp = DateTimeOffset.UtcNow
        };

        await _tableClient.UpsertEntityAsync(entity);
    }

    public async Task<(string? Command, int BingoId)> GetStepAsync(long chatId)
    {
        var notFoundValue = ((string?) null, -1); 
        
        try
        {
            var result = await _tableClient.GetEntityAsync<SessionEntity>(chatId.ToString(), "CurrentCard");
            
            if (result.Value is null)
            {
                return notFoundValue;
                
            }
            
            return (result.Value?.Command, result.Value?.Bingo?? -1);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return notFoundValue;
        }
    }

    public async Task ClearAsync(long chatId)
    {
        await _tableClient.DeleteEntityAsync(chatId.ToString(), "CurrentCard");
    }
}