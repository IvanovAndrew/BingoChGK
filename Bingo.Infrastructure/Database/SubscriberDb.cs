using Postgrest.Attributes;
using Postgrest.Models;

namespace Bingo.Infrastructure.Database;

[Table("chgk.subscribers")]
public class SubscriberDb : BaseModel
{
    [PrimaryKey("id", true)]
    public long Id { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("is_admin")]
    public bool IsAdmin { get; set; }
}