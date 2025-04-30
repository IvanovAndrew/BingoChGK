using Postgrest.Attributes;
using Postgrest.Models;

namespace Bingo.Infrastructure.Database;

[Table("chgk.history")]
public class SessionHistoryDb : BaseModel
{
    [PrimaryKey("subscriber_id", true)]
    public long SubscriberId { get; set; }
    
    [PrimaryKey("bingo_id", true)]
    public int BingoId { get; set; }
    
    [Column("last_shown_date")]
    public DateTime LastShownDate { get; set; }
}